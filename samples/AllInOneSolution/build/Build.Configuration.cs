sealed partial class Build
{
    const string Version = "1.0.0";
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "output";
    readonly AbsolutePath ChangeLogPath = RootDirectory / "Changelog.md";

    protected override void OnBuildInitialized()
    {
        Configurations =
        [
            "Release*",
            "Installer*"
        ];

        Bundles =
        [
            Solution.RevitAddIn
        ];

        InstallersMap = new()
        {
            { Solution.Installer, Solution.RevitAddIn }
        };
    }
}