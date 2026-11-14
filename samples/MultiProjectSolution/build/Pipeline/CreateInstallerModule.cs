using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.RegularExpressions;
using Build.Options;
using Build.Wix;
using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Options;
using ModularPipelines.FileSystem;
using ModularPipelines.Options;
using Shouldly;
using Sourcy.DotNet;
using InstallerOptions = Build.Options.InstallerOptions;

namespace Build.Pipeline;

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

    protected override async Task ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.Value;

        var wixTarget = new FilePath(Projects.RevitAddIn.FullName);
        var wixInstaller = new FilePath(Projects.Installer.FullName);
        var wixToolFolder = await InstallWixAsync(context, cancellationToken);

        await context.Tools.DotNet.BuildAsync(new DotNetBuildOptions
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

        var repository = await context.Tools.Git.Information.GetRequiredInfoAsync(cancellationToken);
        var outputFolder = repository.Root.GetFolder(buildOptions.Value.OutputDirectory);
        if (!outputFolder.Exists)
        {
            await outputFolder.CreateAsync(cancellationToken);
        }

        var manifestFile = await WriteManifestAsync(wixTarget.NameWithoutExtension, contentRoot, targetDirectories, outputFolder, versioning, cancellationToken);

        await context.Shell.RunAsync(
            builderFile.Path,
            [manifestFile.Path],
            new CommandExecutionOptions
            {
                WorkingDirectory = repository.Root.Path,
                EnvironmentVariables = new Dictionary<string, string?>
                {
                    { "PATH", $"{Environment.GetEnvironmentVariable("PATH")};{wixToolFolder}" }
                }
            }, cancellationToken);

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
    private async Task<FilePath> WriteManifestAsync(
        string productName,
        FolderPath contentRoot,
        FolderPath[] targetDirectories,
        FolderPath outputFolder,
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
    private static async Task<FolderPath> InstallWixAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var wixToolFolder = FolderPath.CreateTemporaryFolder();
        await context.Tools.DotNet.Tool.InstallAsync(new DotNetToolInstallOptions("wix")
        {
            Version = "7.*",
            ToolPath = wixToolFolder.Path
        }, cancellationToken: cancellationToken);

        var wixExe = wixToolFolder.GetFile("wix.exe");
        var wixVersion = FileVersionInfo.GetVersionInfo(wixExe.Path).FileVersion!;

        await context.Tools.Wix.EulaAcceptAsync(new WixEulaAcceptOptions("wix7")
        {
            Tool = wixExe.Path
        }, cancellationToken: cancellationToken);

        await context.Tools.Wix.ExtensionAddAsync(new WixExtensionAddOptions($"WixToolset.UI.wixext/{wixVersion}")
        {
            Tool = wixExe.Path,
            Global = true
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
        if (!match.Success)
        {
            return false;
        }

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
