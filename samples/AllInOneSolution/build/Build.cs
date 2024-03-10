using Nuke.Common.Git;
using Nuke.Common.ProjectModel;

sealed partial class Build : NukeBuild
{
    string[] Configurations;
    Project[] Bundles;
    Dictionary<Project, Project> InstallersMap;

    [Secret] [Parameter] string GitHubToken;
    [GitRepository] readonly GitRepository GitRepository;
    [Solution(GenerateProjects = true)] Solution Solution;

    public static int Main() => Execute<Build>(x => x.Compile);
}