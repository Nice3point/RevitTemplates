using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

sealed partial class Build
{
    Target Pack => _ => _
        .TriggeredBy(Compile)
        .Executes(() =>
        {
            foreach (var configuration in GlobBuildConfigurations())
                DotNetPack(settings => settings
                    .SetConfiguration(configuration)
                    .SetVersion(Version)
                    .SetOutputDirectory(ArtifactsDirectory)
                    .SetVerbosity(DotNetVerbosity.Minimal));
        });
}