using Nuke.Common.Git;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

sealed partial class Build
{
    const string NugetApiUrl = "https://api.nuget.org/v3/index.json";
    [Secret] [Parameter] string NugetApiKey;

    Target NuGetPush => _ => _
        .DependsOn(Pack)
        .Requires(() => NugetApiKey)
        .OnlyWhenStatic(() => IsLocalBuild && GitRepository.IsOnMainOrMasterBranch())
        .Executes(() =>
        {
            foreach (var package in ArtifactsDirectory.GlobFiles("*.nupkg"))
                DotNetNuGetPush(settings => settings
                    .SetTargetPath(package)
                    .SetSource(NugetApiUrl)
                    .SetApiKey(NugetApiKey));
        });
}