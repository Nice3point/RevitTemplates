using Nuke.Common.CI.GitHubActions;

sealed partial class Build
{
    [Parameter] [Required] string ReleaseVersion = GitHubActions.Instance?.RefName;

    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "output";
    readonly AbsolutePath ChangeLogPath = RootDirectory / "Changelog.md";
}