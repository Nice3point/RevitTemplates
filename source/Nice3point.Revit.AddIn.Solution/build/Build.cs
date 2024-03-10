#if (!NoPipeline)
using Nuke.Common.Git;
#endif
using Nuke.Common.ProjectModel;

sealed partial class Build : NukeBuild
{
    string[] Configurations;
#if (bundle)
    Project[] Bundles;
#endif
#if (installer)
    Dictionary<Project, Project> InstallersMap;
#endif

#if (GitHubPipeline)
    [Secret] [Parameter] string GitHubToken;
#endif
#if (!NoPipeline)
    [GitRepository] readonly GitRepository GitRepository;
#endif
    [Solution(GenerateProjects = true)] Solution Solution;

    public static int Main() => Execute<Build>(x => x.Compile);
}