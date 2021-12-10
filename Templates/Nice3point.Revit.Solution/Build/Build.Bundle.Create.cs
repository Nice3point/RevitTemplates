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
            Directory.CreateDirectory(contentDirectory);

            foreach (var directoryGroup in buildDirectories)
            {
                foreach (var directoryInfo in directoryGroup.ToList())
                {
                    if (!directoryInfo.Name.Contains(BuildConfiguration)) continue;
                    if (BundleConfiguration.Length != 0 || directoryGroup.Key.Length != 0)
                    {
                        if (BundleConfiguration.Length == 0 || !directoryInfo.Name.EndsWith(BundleConfiguration)) continue;
                    }

                    var version = versionPatter.Match(directoryInfo.Name).Value;
                    if (string.IsNullOrEmpty(version))
                    {
                        Log.Warning("Missing version label for directory: \"{Directory}\"", directoryInfo.Name);
                        continue;
                    }

                    var buildDirectory = contentDirectory / version;
                    Log.Information("Added {Version} version files: ", version);
                    CopyAssemblies(directoryInfo.FullName, buildDirectory);
                }
            }

            if (!Directory.EnumerateDirectories(contentDirectory).Any())
                throw new Exception($"No configuration found to create a bundle. Check that the solution configuration end with \"{BundleConfiguration}\"");
        });

    static void CopyAssemblies(string sourcePath, string targetPath)
    {
        foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        foreach (var filePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            Log.Information("{Assembly}", filePath);
            File.Copy(filePath, filePath.Replace(sourcePath, targetPath), true);
        }
    }
}