using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Nuke.Common;
using Nuke.Common.Git;

partial class Build
{
    Target CreateBundle => _ => _
        .TriggeredBy(Compile)
<!--#if (HazPipeline)
        .OnlyWhenStatic(() => IsLocalBuild || GitRepository.IsOnMainOrMasterBranch())
#endif-->
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
                        CopyFilesContent(directoryInfo.FullName, buildDirectory);
                        var assemblies = Directory.GetFiles(directoryInfo.FullName, "*", SearchOption.AllDirectories);
                        foreach (var assembly in assemblies)
                        {
                            Logger.Normal($"Added {version} version file: {assembly}");
                        }
                    }
                }
            }

            if (!Directory.Exists(contentDirectory))
                throw new Exception($"No configuration found to create a bundle. Check that the solution configuration end with \"{BundleConfiguration}\".");
        });

    void CopyFilesContent(string sourcePath, string targetPath)
    {
        foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
    }
}