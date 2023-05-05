using Nuke.Common;
using static Nuke.Common.IO.FileSystemTasks;

partial class Build
{
    Target Clean => _ => _
        .OnlyWhenStatic(() => IsLocalBuild)
        .Executes(() =>
        {
<!--#if (Bundle || Installer || GitHubPipeline)
            EnsureCleanDirectory(ArtifactsDirectory);

#endif-->
            foreach (var project in Solution.AllProjects.Where(project => project != Solution.Build))
                EnsureCleanDirectory(project.Directory / "bin");
        });
}