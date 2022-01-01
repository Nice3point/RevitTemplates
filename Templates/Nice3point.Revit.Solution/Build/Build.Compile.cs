using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build
{
    Target Compile => _ => _
        .TriggeredBy(Cleaning)
        .Executes(() =>
        {
<!--#if (Installer)
            var configurations = GetConfigurations(BuildConfiguration, InstallerConfiguration);
<!--#else
            var configurations = GetConfigurations(BuildConfiguration);
#endif-->
            configurations.ForEach(configuration =>
            {
                DotNetBuild(settings => settings
                    .SetProcessToolPath(MsBuildPath.Value)
                    .SetConfiguration(configuration)
                    .SetVerbosity(DotNetVerbosity.Minimal));
            });
        });
}