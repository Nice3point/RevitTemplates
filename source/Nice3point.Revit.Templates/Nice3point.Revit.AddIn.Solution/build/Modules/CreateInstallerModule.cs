using Build.Options;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.FileSystem;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;
using ModularPipelines.Options;
using Shouldly;
using Sourcy.DotNet;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.RegularExpressions;
using File = ModularPipelines.FileSystem.File;
using InstallerOptions = Build.Options.InstallerOptions;

namespace Build.Modules;

/// <summary>
///     Represents the pipeline step that builds the MSI installer packages of the add-in.
/// </summary>
[DependsOn<ResolveVersioningModule>]
[DependsOn<CompileProjectModule>]
public sealed partial class CreateInstallerModule(IOptions<BuildOptions> buildOptions, IOptions<InstallerOptions> installerOptions) : Module
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.ValueOrDefault!;

        var wixTarget = new File(Projects.Nice3point_Revit_AddIn__1.FullName);
        var wixInstaller = new File(Projects.Installer.FullName);
        var wixToolFolder = await InstallWixAsync(context, cancellationToken);

        await context.DotNet().Build(new DotNetBuildOptions
        {
            ProjectSolution = wixInstaller.Path,
            Configuration = "Release"
        }, cancellationToken: cancellationToken);

        var builderFile = wixInstaller.Folder!
            .GetFolder("bin")
            .FindFile(file => file.NameWithoutExtension == wixInstaller.NameWithoutExtension && file.Extension == ".exe");

        builderFile.ShouldNotBeNull($"Cannot create the installer. The executable of the '{wixInstaller.NameWithoutExtension}' project was not found in '{wixInstaller.Folder!.Path}'.");

        var contentRoot = wixTarget.Folder!.GetFolder("bin");
        var targetDirectories = contentRoot
            .GetFolders(folder => folder.Name == "publish")
            .ToArray();

        targetDirectories.ShouldNotBeEmpty($"Cannot create the installer. No publish output was found in '{contentRoot.Path}'. Set 'PublishAddin' to 'true' in the add-in project.");

        var outputFolder = context.Git().RootDirectory.GetFolder(buildOptions.Value.OutputDirectory);
        if (!outputFolder.Exists)
        {
            await outputFolder.CreateAsync(cancellationToken);
        }

        var manifestFile = await WriteManifestAsync(wixTarget.NameWithoutExtension, contentRoot, targetDirectories, outputFolder, versioning, cancellationToken);

        await context.Shell.Command.ExecuteCommandLineTool(
            new GenericCommandLineToolOptions(builderFile.Path)
            {
                Arguments = [manifestFile.Path]
            },
            new CommandExecutionOptions
            {
                WorkingDirectory = context.Git().RootDirectory,
                EnvironmentVariables = new Dictionary<string, string?>
                {
                    { "PATH", $"{Environment.GetEnvironmentVariable("PATH")};{wixToolFolder}" }
                }
            }, cancellationToken: cancellationToken);

        await wixToolFolder.DeleteAsync(cancellationToken);

        var outputFiles = outputFolder.GetFiles(file => file.Extension == ".msi").ToArray();
        outputFiles.ShouldNotBeEmpty($"Cannot create the installer. The installer project wrote no MSI packages to '{outputFolder.Path}'.");

        foreach (var outputFile in outputFiles)
        {
            context.Summary.KeyValue("Artifacts", "Installer", outputFile.Path);
        }
    }

    /// <summary>
    ///     Writes the installer manifest for every compiled Revit configuration.
    /// </summary>
    private async Task<File> WriteManifestAsync(
        string productName,
        Folder contentRoot,
        Folder[] targetDirectories,
        Folder outputFolder,
        ResolveVersioningResult versioning,
        CancellationToken cancellationToken)
    {
        var content = targetDirectories
            .Select(targetDirectory =>
            {
                TryParseVersion(targetDirectory.Path, out var revitVersion)
                    .ShouldBeTrue($"Cannot create the installer. The Revit version of '{targetDirectory.Path}' cannot be resolved. Name the build configuration after the Revit version, such as 'Release.R26'.");

                var basePath = Path.GetRelativePath(contentRoot.Path, targetDirectory.Path);

                return new
                {
                    RevitVersion = int.Parse(revitVersion),
                    Files = new[]
                    {
                        new
                        {
                            Role = "payload",
                            BasePath = basePath,
                            Include = new[] { "**" },
                            Exclude = new[] { "**/*.addin", "**/*.pdb" }
                        },
                        new
                        {
                            Role = "addin",
                            BasePath = basePath,
                            Include = new[] { "**/*.addin" },
                            Exclude = Array.Empty<string>()
                        }
                    }
                };
            })
            .ToArray();

        var manifest = new
        {
            ProductName = productName,
            ProductVersion = versioning.VersionPrefix,
            UpgradeCode = installerOptions.Value.UpgradeCode!.Value,
            ReleaseVersion = versioning.Version,
            OutputDirectory = outputFolder.Path,
            Content = content
        };

        var manifestFile = contentRoot.GetFile("installer.manifest.json");
        var manifestContent = JsonSerializer.Serialize(manifest, SerializerOptions);
        await manifestFile.WriteAsync(manifestContent, cancellationToken);

        return manifestFile;
    }

    /// <summary>
    ///     Installs the WiX toolset to a temporary folder.
    /// </summary>
    private static async Task<Folder> InstallWixAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var wixToolFolder = Folder.CreateTemporaryFolder();
        await context.DotNet().Tool.Execute(new DotNetToolOptions
        {
            Arguments = ["install", "wix", "--version", "7.*", "--tool-path", wixToolFolder.Path]
        }, cancellationToken: cancellationToken);

        var wixExe = wixToolFolder.GetFile("wix.exe");
        var wixVersion = FileVersionInfo.GetVersionInfo(wixExe.Path).FileVersion!;

        await context.Shell.Command.ExecuteCommandLineTool(
            new GenericCommandLineToolOptions(wixExe.Path)
            {
                Arguments = ["eula", "accept", "wix7"]
            }, cancellationToken: cancellationToken);

        await context.Shell.Command.ExecuteCommandLineTool(
            new GenericCommandLineToolOptions(wixExe.Path)
            {
                Arguments = ["extension", "add", "-g", $"WixToolset.UI.wixext/{wixVersion}"]
            }, cancellationToken: cancellationToken);

        return wixToolFolder;
    }

    /// <summary>
    ///     Converts the last number in the specified path to a four-digit Revit version.
    /// </summary>
    /// <example>
    ///     bin\Release.R26\publish → 2026 <br />
    ///     bin\Release.R2026\publish → 2026
    /// </example>
    private static bool TryParseVersion(string input, [NotNullWhen(true)] out string? version)
    {
        version = null;
        var match = VersionRegex().Match(input);
        if (!match.Success) return false;

        switch (match.Value.Length)
        {
            case 4:
                version = match.Value;
                return true;
            case 2:
                version = $"20{match.Value}";
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    ///     Gets the regular expression that matches the last number in a path.
    /// </summary>
    [GeneratedRegex(@"(\d+)(?!.*\d)")]
    private static partial Regex VersionRegex();
}
