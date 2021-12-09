using System.Text.RegularExpressions;
using Nuke.Common;
<!--#if (!NoPipeline)
using Nuke.Common.Git;
#endif-->
using Serilog;

partial class Build
{
    Target CreateBundle => _ => _
        .TriggeredBy(Compile)
<!--#if (!NoPipeline)
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
                            Log.Warning("Missing version label for directory: \"{Directory}\".", directoryInfo.Name);
                            continue;
                        }

                        var buildDirectory = contentDirectory / version;
                        CopyFilesContent(directoryInfo.FullName, buildDirectory);
                        var assemblies = Directory.GetFiles(directoryInfo.FullName, "*", SearchOption.AllDirectories);
                        Log.Information("Added {Version} version files: ", version);
                        foreach (var assembly in assemblies) Log.Information("\t{Assembly}", assembly);
                    }
                }
            }

            if (!Directory.Exists(contentDirectory))
                throw new Exception($"No configuration found to create a bundle. Check that the solution configuration end with \"{BundleConfiguration}\".");
        });

    static void CopyFilesContent(string sourcePath, string targetPath)
    {
        foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
    }
}