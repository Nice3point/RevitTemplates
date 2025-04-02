using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

sealed partial class Build
{
    /// <summary>
    ///     Compile all solution configurations.
    /// </summary>
    Target Compile => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            foreach (var configuration in GlobBuildConfigurations())
            {
                DotNetBuild(settings => settings
                    .SetProjectFile(Solution)
                    .SetConfiguration(configuration)
                    .SetVersion(ReleaseVersionNumber)
                    .SetVerbosity(DotNetVerbosity.minimal));
            }
        });
}