using Autodesk.PackageBuilder;
using System.Xml.Linq;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Utilities;

sealed partial class Build
{
    Target CreateBundle => _ => _
        .DependsOn(Compile)
        .OnlyWhenStatic(() => IsLocalBuild || GitRepository.IsOnMainOrMasterBranch())
        .Executes(() =>
        {
            foreach (var project in Bundles)
            {
                Log.Information("Project: {Name}", project.Name);

                var directories = Directory.GetDirectories(project.Directory, "* Release *", SearchOption.AllDirectories);
                Assert.NotEmpty(directories, "No files were found to create a bundle");

                var bundleRoot = ArtifactsDirectory / project.Name;
                var bundlePath = bundleRoot / $"{project.Name}.bundle";
                var manifestPath = bundlePath / "PackageContents.xml";
                var contentsDirectory = bundlePath / "Contents";
                foreach (var path in directories)
                {
                    var version = YearRegex.Match(path).Value;

                    Log.Information("Bundle files for version {Version}:", version);
                    CopyAssemblies(path, contentsDirectory / version);
                }

                GenerateManifest(project, directories, manifestPath);
                CompressFolder(bundleRoot);
            }
        });

    void GenerateManifest(Project project, string[] directories, AbsolutePath manifestDirectory)
    {
        BuilderUtils.Build<PackageContentsBuilder>(builder =>
        {
            var versions = directories.Select(path => YearRegex.Match(path).Value).Select(int.Parse);
            var company = GetConfigurationValue(project, config => config.Name == "VendorId");
            var email = GetConfigurationValue(project, config => config.Name == "VendorEmail");

            builder.ApplicationPackage.Create()
                .ProductType(ProductTypes.Application)
                .AutodeskProduct(AutodeskProducts.Revit)
                .Name(Solution.Name)
                .AppVersion(Version);

            builder.CompanyDetails.Create(company)
                .Email(email);

            foreach (var version in versions)
            {
                builder.Components.CreateEntry($"Revit {version}")
                    .RevitPlatform(version)
                    .AppName(project.Name)
                    .ModuleName($"./Contents/{version}/{project.Name}.addin");
            }
        }, manifestDirectory);
    }

    string GetConfigurationValue(Project project, Func<XElement, bool> filter)
    {
        var defaultValue = string.Empty;
        var configPath = project.Directory.GetFiles("*.addin").FirstOrDefault();

        if (configPath is null) return defaultValue;

        var configDocument = configPath.ReadXml();
        if (configDocument.Root is null) return defaultValue;

        var sectionElement = configDocument.Root.Elements().FirstOrDefault();
        if (sectionElement is null) return defaultValue;

        var configElement = sectionElement.Elements().FirstOrDefault(filter);
        if (configElement is null) return defaultValue;

        return configElement.Value;
    }

    static void CompressFolder(AbsolutePath bundleRoot)
    {
        var bundleName = bundleRoot.WithExtension(".zip");
        bundleRoot.CompressTo(bundleName);
        bundleRoot.DeleteDirectory();

        Log.Information("Compressing into a Zip: {Name}", bundleName);
    }

    static void CopyAssemblies(string sourcePath, string targetPath)
    {
        foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

        foreach (var filePath in Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories))
        {
            Log.Information("{Assembly}", filePath);
            File.Copy(filePath, filePath.Replace(sourcePath, targetPath), true);
        }
    }
}