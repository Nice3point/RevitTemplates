using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Autodesk.PackageBuilder;
using Build.Options;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.FileSystem;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;
using Shouldly;
using Sourcy.DotNet;
using File = ModularPipelines.FileSystem.File;

namespace Build.Modules;

/// <summary>
///     Represents the pipeline step that packs the add-in into an Autodesk application bundle.
/// </summary>
[DependsOn<ResolveVersioningModule>]
[DependsOn<CompileProjectModule>]
public sealed partial class CreateBundleModule(IOptions<BuildOptions> buildOptions, IOptions<BundleOptions> bundleOptions) : Module
{
    protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.ValueOrDefault!;

        var bundleTarget = new File(Projects.RevitAddIn.FullName);
        var contentRoot = bundleTarget.Folder!.GetFolder("bin");
        var targetDirectories = contentRoot
            .GetFolders(folder => folder.Name == "publish")
            .ToArray();

        targetDirectories.ShouldNotBeEmpty($"Cannot create the bundle. No publish output was found in '{contentRoot.Path}'. Set 'PublishAddin' to 'true' in the add-in project.");

        var outputFolder = context.Git().RootDirectory.GetFolder(buildOptions.Value.OutputDirectory);
        var bundleFolder = outputFolder.CreateFolder($"{bundleTarget.NameWithoutExtension}.bundle");
        var contentFolder = bundleFolder.CreateFolder("Contents");
        var manifestFile = bundleFolder.GetFile("PackageContents.xml");

        PackFiles(targetDirectories, contentFolder);
        GenerateManifest(bundleTarget, targetDirectories, manifestFile, versioning);

        var outputFile = outputFolder.GetFile($"{bundleFolder.Name}.zip");
        context.Files.Zip.ZipFolder(bundleFolder, outputFile.Path);
        await bundleFolder.DeleteAsync(cancellationToken);

        context.Summary.KeyValue("Artifacts", "Bundle", outputFile.Path);
    }

    /// <summary>
    ///     Copies the publish output of every Revit version to the bundle contents.
    /// </summary>
    private static void PackFiles(Folder[] targetDirectories, Folder contentFolder)
    {
        foreach (var targetDirectory in targetDirectories)
        {
            var version = ResolveRevitVersion(targetDirectory);
            var versionFolder = contentFolder.CreateFolder(version);
            foreach (var filePath in targetDirectory.GetFiles(file => file.Exists))
            {
                var relativePath = Path.GetRelativePath(targetDirectory.Path, filePath.Path);
                var destinationPath = versionFolder.GetFile(relativePath);
                if (!destinationPath.Folder!.Exists)
                {
                    destinationPath.Folder!.Create();
                }

                filePath.CopyTo(destinationPath.Path);
            }
        }
    }

    /// <summary>
    ///     Creates the <c>PackageContents.xml</c> manifest of the bundle.
    /// </summary>
    private void GenerateManifest(File bundleTarget, Folder[] targetDirectories, File manifestFile, ResolveVersioningResult versioning)
    {
        BuilderUtils.Build<PackageContentsBuilder>(builder =>
        {
            builder.ApplicationPackage.Create()
                .ProductType(ProductTypes.Application)
                .AutodeskProduct(AutodeskProducts.Revit)
                .Name(bundleTarget.NameWithoutExtension)
                .AppVersion(versioning.Version);

            builder.CompanyDetails.Create(bundleOptions.Value.VendorName)
                .Email(bundleOptions.Value.VendorEmail)
                .Url(bundleOptions.Value.VendorUrl);

            foreach (var targetDirectory in targetDirectories)
            {
                var version = ResolveRevitVersion(targetDirectory);
                var addinManifests = targetDirectory.GetFiles(file => file.Extension == ".addin");
                foreach (var addinManifest in addinManifests)
                {
                    var relativePath = Path.GetRelativePath(targetDirectory.Path, addinManifest.Path);

                    builder.Components.CreateEntry($"Revit {version}")
                        .RevitPlatform(int.Parse(version))
                        .AppName(bundleTarget.NameWithoutExtension)
                        .ModuleName($"./Contents/{version}/{relativePath}");
                }
            }
        }, manifestFile);
    }

    /// <summary>
    ///     Gets the four-digit Revit version of the specified publish directory.
    /// </summary>
    private static string ResolveRevitVersion(Folder targetDirectory)
    {
        TryParseVersion(targetDirectory.Path, out var version)
            .ShouldBeTrue($"Cannot create the bundle. The Revit version of '{targetDirectory.Path}' cannot be resolved. Name the build configuration after the Revit version, such as 'Release.R26'.");

        return version;
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
