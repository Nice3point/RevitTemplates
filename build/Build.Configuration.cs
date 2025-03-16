sealed partial class Build
{
    [Parameter] string ReleaseVersion;

    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "output";
    readonly AbsolutePath ChangelogPath = RootDirectory / "Changelog.md";
    
    protected override void OnBuildInitialized()
    {
        ReleaseVersion ??= GitRepository.Tags.SingleOrDefault();
    }
}