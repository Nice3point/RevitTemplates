using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

sealed partial class Build
{
    Target Pack => _ => _
        .DependsOn(Clean)
        .Requires(() => ReleaseVersion)
        .Executes(() =>
        {
            DotNetPack(settings => settings
                .SetConfiguration("Release")
                .SetVersion(ReleaseVersion)
                .SetOutputDirectory(ArtifactsDirectory)
                .SetVerbosity(DotNetVerbosity.minimal)
                .SetPackageReleaseNotes(CreateNugetChangelog()));
        });
}