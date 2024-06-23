sealed partial class Build
{
    const string Version = "4.0.5";
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "output";
    readonly AbsolutePath ChangeLogPath = RootDirectory / "Changelog.md";

    protected override void OnBuildInitialized()
    {
        Configurations = 
        [
            "Release"
        ];
    }
}