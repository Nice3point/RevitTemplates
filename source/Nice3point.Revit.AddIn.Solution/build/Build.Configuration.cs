sealed partial class Build
{
    const string Version = "1.0.0";
#if (Bundle || Installer || GitHubPipeline)
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "output";
#endif
#if (GitHubPipeline)
    readonly AbsolutePath ChangeLogPath = RootDirectory / "Changelog.md";
#endif

    protected override void OnBuildInitialized()
    {
        Configurations =
        [
            "Release*",
#if (Installer)
            "Installer*"
#endif
        ];
#if (Bundle)

        Bundles =
        [
            Solution.Nice3point.Revit.AddIn.Solution
        ];
#endif
#if (Installer)

        InstallersMap = new()
        {
            {Solution.Installer, Solution.Nice3point.Revit.AddIn.Solution}
        };
#endif
    }
}