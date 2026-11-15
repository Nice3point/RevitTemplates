using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.RegularExpressions;
using Build.Pipeline.Compilation;
using Build.Pipeline.Versioning;
using Build.Wix;
using Installer.Manifest;
using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Options;
using ModularPipelines.FileSystem;
using ModularPipelines.Options;
using Shouldly;
using Sourcy.DotNet;

namespace Build.Pipeline.Packaging;

/// <summary>
///     Represents the pipeline step that builds the MSI installer packages of the add-in.
/// </summary>
[DependsOn<ResolveVersioningModule>]
[DependsOn<CompileProjectModule>]
public sealed partial class CreateInstallerModule(IOptions<BuildOptions> buildOptions, IOptions<InstallerOptions> installerOptions) : Module
{
    /// <inheritdoc />
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
        var manifest = new InstallerManifest
        {
            ProductName = productName,
            ProductVersion = Version.Parse(versioning.VersionPrefix),
            UpgradeCode = installerOptions.Value.UpgradeCode!.Value,
            ReleaseVersion = versioning.Version,
            OutputDirectory = outputFolder.Path,
            Content = [.. targetDirectories.Select(targetDirectory => CreateAddinContent(contentRoot, targetDirectory))]
        };

        var manifestFile = contentRoot.GetFile("installer.manifest.json");
        var manifestContent = JsonSerializer.Serialize(manifest, InstallerJsonContext.Default.InstallerManifest);
        await manifestFile.WriteAsync(manifestContent, cancellationToken);

        return manifestFile;
    }

    /// <summary>
    ///     Creates the manifest entry of the add-in files in the specified publish directory.
    /// </summary>
    /// <remarks>The add-in manifest is installed after the files it refers to.</remarks>
    private static InstallerManifest.AddinContent CreateAddinContent(FolderPath contentRoot, FolderPath targetDirectory)
    {
        TryParseVersion(targetDirectory.Path, out var revitVersion)
            .ShouldBeTrue($"Cannot create the installer. The Revit version of '{targetDirectory.Path}' cannot be resolved. Name the build configuration after the Revit version, such as 'Release.R26'.");

        var basePath = Path.GetRelativePath(contentRoot.Path, targetDirectory.Path);

        return new InstallerManifest.AddinContent
        {
            RevitVersion = int.Parse(revitVersion),
            Files =
            [
                new InstallerManifest.FileSet
                {
                    Role = FileSetRoles.Payload,
                    BasePath = basePath,
                    Include = ["**"],
                    Exclude = ["**/*.addin", "**/*.pdb"]
                },
                new InstallerManifest.FileSet
                {
                    Role = FileSetRoles.AddinManifest,
                    BasePath = basePath,
                    Include = ["**/*.addin"]
                }
            ]
        };
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
    /// <example>Both <c>bin\Release.R26\publish</c> and <c>bin\Release.R2026\publish</c> return <c>2026</c>.</example>
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
