sealed partial class Build
{
    const string Version = "3.2.0";
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "output";
    readonly AbsolutePath ChangeLogPath = RootDirectory / "Changelog.md";

    protected override void OnBuildCreated()
    {
        Configurations = new[]
        {
            "Release"
        };
    }
}