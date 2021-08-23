using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using Nuke.Common;

partial class Build
{
    Target CreateBundle => _ => _
        .TriggeredBy(Compile)
        .Executes(() =>
        {
            var project = BuilderExtensions.GetProject(Solution, MainProjectName);
            var releaseAddInDirectories = GetReleaseAddInDirectories();
            var contentDirectory = project.GetBundleDirectory(ArtifactsDirectory) / "Contents";
            var versionPatter = new Regex(@"\d+");

            var foundedDirectories = 0;
            foreach (var directoryGroup in releaseAddInDirectories)
            {
                var directories = directoryGroup.ToList();
                var dirName = directories[0].Name;
                if (dirName.Contains(BuildConfiguration) && dirName.EndsWith(BundleSuffixConfiguration))
                {
                    Directory.CreateDirectory(contentDirectory);
                    IterateDirectoriesRegex(directories, versionPatter, (directoryInfo, version) =>
                    {
                        var buildDirectory = contentDirectory / version;
                        Logger.Normal($"Copy files from: {directoryInfo.FullName} to {buildDirectory}");
                        CopyFilesContent(directoryInfo.FullName, buildDirectory);
                    });
                    foundedDirectories++;
                }
            }

            if (foundedDirectories == 0) Logger.Warn($"No \"{BundleSuffixConfiguration}\" configurations found in:{project.GetBinDirectory()}");
        });

    Target ZipBundle => _ => _
        .TriggeredBy(CreateBundle)
        .Produces(ArtifactsDirectory / "*.zip")
        .Executes(() =>
        {
            var project = BuilderExtensions.GetProject(Solution, MainProjectName);
            var bundleDirectory = project.GetBundleDirectory(ArtifactsDirectory);
            if (Directory.Exists(bundleDirectory))
            {
                var archiveName = $"{bundleDirectory}.zip";
                Logger.Normal($"Archive creation: {bundleDirectory}\\{archiveName}");
                ZipFile.CreateFromDirectory(bundleDirectory, archiveName);
            }
            else
            {
                Logger.Warn($"Directory not found for archiving: {bundleDirectory}");
            }
        });

    void CopyFilesContent(string sourcePath, string targetPath)
    {
        foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
    }
}