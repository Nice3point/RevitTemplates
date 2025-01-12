sealed partial class Build
{
    [Parameter] string ReleaseVersion = EnvironmentInfo.GetVariable("PUBLISH_VERSION");
    
#if (bundle || installer || GitHubPipeline)
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
#if (installer)
            "Installer*"
#endif
        ];
#if (bundle)

        Bundles =
        [
            Solution.Nice3point.Revit.AddIn
        ];
#endif
#if (installer)

        InstallersMap = new()
        {
            {Solution.Installer, Solution.Nice3point.Revit.AddIn}
        };
#endif
    }
}