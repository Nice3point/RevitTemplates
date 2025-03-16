using Nuke.Common.Tools.DotNet;
using Nuke.Common.ProjectModel;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

sealed partial class Build
{
    /// <summary>
    ///     Clean projects with dependencies.
    /// </summary>
    Target Clean => _ => _
        .OnlyWhenStatic(() => IsLocalBuild)
        .Executes(() =>
        {
            Project[] excludedProjects =
            [
                Solution.Automation.Build
            ];
            
#if (HasArtifacts)
            CleanDirectory(ArtifactsDirectory);
#endif
            foreach (var project in Solution.AllProjects)
            {
                if (excludedProjects.Contains(project)) continue;
                
                CleanDirectory(project.Directory / "bin");
                CleanDirectory(project.Directory / "obj");
            }

            foreach (var configuration in GlobBuildConfigurations())
            {
                DotNetClean(settings => settings
                    .SetConfiguration(configuration)
                    .SetProject(Solution)
                    .SetVerbosity(DotNetVerbosity.minimal)
                    .EnableNoLogo());
            }
        });

    /// <summary>
    ///     Clean and log the specified directory.
    /// </summary>
    static void CleanDirectory(AbsolutePath path)
    {
        Log.Information("Cleaning directory: {Directory}", path);
        path.CreateOrCleanDirectory();
    }
}