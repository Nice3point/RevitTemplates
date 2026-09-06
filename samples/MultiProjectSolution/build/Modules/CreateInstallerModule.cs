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
///     Create the .msi installer.
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

        var wixTarget = new File(Projects.RevitAddIn.FullName);
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

        builderFile.ShouldNotBeNull($"No installer builder was found for the project: {wixInstaller.NameWithoutExtension}");

        var contentRoot = wixTarget.Folder!.GetFolder("bin");
        var targetDirectories = contentRoot
            .GetFolders(folder => folder.Name == "publish")
            .ToArray();

        targetDirectories.ShouldNotBeEmpty("No content were found to create an installer");

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
                    {"PATH", $"{Environment.GetEnvironmentVariable("PATH")};{wixToolFolder}"}
                }
            }, cancellationToken: cancellationToken);

        var outputFiles = outputFolder.GetFiles(file => file.Extension == ".msi").ToArray();
        outputFiles.ShouldNotBeEmpty("Failed to create an installer");

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
                    .ShouldBeTrue($"Could not parse version from directory name: {targetDirectory.Path}");

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
                            Include = new[] {"**"},
                            Exclude = new[] {"**/*.addin", "**/*.pdb"}
                        },
                        new
                        {
                            Role = "addin",
                            BasePath = basePath,
                            Include = new[] {"**/*.addin"},
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
    ///     Installs the WiX toolset required for building installers.
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
    ///     Parse a version string from the given input.
    /// </summary>
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
    ///     A regular expression that captures the Revit version at the end of a build output path.
    /// </summary>
    [GeneratedRegex(@"(\d+)(?!.*\d)")]
    private static partial Regex VersionRegex();
}
