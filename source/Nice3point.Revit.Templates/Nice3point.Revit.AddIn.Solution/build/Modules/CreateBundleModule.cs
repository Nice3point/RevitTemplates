using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Autodesk.PackageBuilder;
using Build.Options;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.FileSystem;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using Shouldly;
using Sourcy.DotNet;
using File = ModularPipelines.FileSystem.File;

namespace Build.Modules;

/// <summary>
///     Create the Autodesk .bundle package.
/// </summary>
[DependsOn<ResolveVersioningModule>]
[DependsOn<CompileProjectModule>]
public sealed partial class CreateBundleModule(IOptions<BundleOptions> bundleOptions) : Module<CommandResult>
{
    protected override async Task<CommandResult?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.Value!;

        var bundleTarget = new File(Projects.Nice3point.Revit.AddIn.FullName);
        var targetDirectories = bundleTarget.Folder!
            .GetFolder("bin")
            .GetFolders(folder => folder.Name == "publish")
            .ToArray();

        targetDirectories.ShouldNotBeEmpty("No content were found to create a bundle");

        var outputFolder = context.Git().RootDirectory.GetFolder("output");
        var bundleFolder = outputFolder.CreateFolder($"{bundleTarget.NameWithoutExtension}.bundle");
        var contentFolder = bundleFolder.CreateFolder("Content");
        var manifestFile = bundleFolder.GetFile("PackageContents.xml");

        PackFiles(targetDirectories, contentFolder);
        GenerateManifest(bundleTarget, targetDirectories, manifestFile, versioning);

        context.Zip.ZipFolder(bundleFolder, outputFolder.GetFile($"{bundleFolder.Name}.zip").Path);
        bundleFolder.Delete();

        return await NothingAsync();
    }

    private static void PackFiles(Folder[] targetDirectories, Folder contentFolder)
    {
        foreach (var targetDirectory in targetDirectories)
        {
            if (!TryParseVersion(targetDirectory.Path, out var version))
            {
                throw new Exception($"Could not parse version from directory name: {targetDirectory.Path}");
            }

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
    ///     Generate the Autodesk manifest.
    /// </summary>
    private void GenerateManifest(File bundleTarget, Folder[] targetDirectories, File manifestDirectory, ResolveVersioningResult versioning)
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
                if (!TryParseVersion(targetDirectory.Path, out var version))
                {
                    throw new Exception($"Could not parse version from directory name: {targetDirectory.Path}");
                }

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
        }, manifestDirectory);
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
    ///     A regular expression to match the last sequence of numeric characters in a string.
    /// </summary>
    [GeneratedRegex(@"(\d+)(?!.*\d)")]
    private static partial Regex VersionRegex();
}