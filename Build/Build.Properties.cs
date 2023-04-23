partial class Build
{
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "output";

    readonly string[] Configurations =
    {
        "Release"
    };

    readonly Dictionary<string, string> VersionMap = new()
    {
        {"Release", "3.0.2"}
    };
}