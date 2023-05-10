using Nuke.Common.IO;

sealed partial class Build
{
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "output";

    protected override void OnBuildCreated()
    {
        Configurations = new[]
        {
            "Release"
        };

        VersionMap = new()
        {
            {"Release", "3.1.0"}
        };
    }
}