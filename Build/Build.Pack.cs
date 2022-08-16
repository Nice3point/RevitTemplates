using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build
{
    Target Pack => _ => _
        .TriggeredBy(Compile)
        .Executes(() =>
        {
            var configurations = GetConfigurations(BuildConfiguration);
            configurations.ForEach(configuration =>
            {
                DotNetPack(settings => settings
                    .SetConfiguration(configuration)
                    .SetVersion(PackageVersion)
                    .SetOutputDirectory(ArtifactsDirectory)
                    .SetVerbosity(DotNetVerbosity.Minimal));
            });
        });
}