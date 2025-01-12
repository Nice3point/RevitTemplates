sealed partial class Build
{
    [Parameter] [Required] string ReleaseVersion = EnvironmentInfo.GetVariable("PUBLISH_VERSION");

    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "output";
    readonly AbsolutePath ChangeLogPath = RootDirectory / "Changelog.md";
}