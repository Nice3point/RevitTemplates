using Nuke.Common;
using Nuke.Common.Git;
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
            foreach (var project in Bundles)
            {
                Log.Information("Project: {Name}", project.Name);

                var publishDirectories = Directory.GetDirectories(project.Directory, "Publish*", SearchOption.AllDirectories);
                if (publishDirectories.Length == 0) throw new Exception("No files were found to create a bundle");

                var contentsDirectory = ArtifactsDirectory / $"{project.Name}.bundle" / "Contents";
                foreach (var folder in publishDirectories)
                {
                    var version = YearRegex.Match(folder).Value;

                    Log.Information("Bundle files for version {Version}:", version);
                    CopyAssemblies(folder, contentsDirectory / version);
                }
            }
        });

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