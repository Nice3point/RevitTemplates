using System;
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
            var versionPatter = new Regex(@"\d+");
            var contentDirectory = Solution.GetBundleDirectory(ArtifactsDirectory) / "Contents";
            var buildDirectories = GetBuildDirectories();

            foreach (var directoryGroup in buildDirectories)
            {
                var directories = directoryGroup.ToList();
                if (directories.All(info => info.Name.Contains(BuildConfiguration) && info.Name.EndsWith(BundleConfiguration)))
                {
                    Directory.CreateDirectory(contentDirectory);
                    foreach (var directoryInfo in directories)
                    {
                        var version = versionPatter.Match(directoryInfo.Name).Value;
                        if (string.IsNullOrEmpty(version))
                        {
                            Logger.Warn($"Missing version label for directory: \"{directoryInfo.Name}\".");
                            continue;
                        }

                        var buildDirectory = contentDirectory / version;
                        Logger.Normal($"Copy files from: {directoryInfo.FullName} to {buildDirectory}.");
                        CopyFilesContent(directoryInfo.FullName, buildDirectory);
                    }
                }
            }

            if (!Directory.Exists(contentDirectory))
                throw new Exception($"No configuration found to create a bundle. Check that the solution configuration end with \"{BundleConfiguration}\".");
        });

    Target ZipBundle => _ => _
        .TriggeredBy(CreateBundle)
        .Produces(ArtifactsDirectory / "*.zip")
        .Executes(() =>
        {
            var bundleDirectory = Solution.GetBundleDirectory(ArtifactsDirectory);
            if (Directory.Exists(bundleDirectory))
            {
                var archiveName = $"{bundleDirectory}.zip";
                Logger.Normal($"Archive creation: {archiveName}");
                ZipFile.CreateFromDirectory(bundleDirectory, archiveName);
                Logger.Normal($"Deletion directory: {bundleDirectory}");
                Directory.Delete(bundleDirectory, true);
            }
            else
            {
                throw new Exception($"Directory not found for archiving: {bundleDirectory}");
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