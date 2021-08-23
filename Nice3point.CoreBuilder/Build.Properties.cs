partial class Build
{
    const string MainProjectName = "Nice3point.FrameworkAddIn"; /*caret*/
    const string InstallerProjectName = "Installer";

    public const string BuildConfiguration = "Release";
    public const string InstallerConfiguration = "Installer";
    public const string BundleSuffixConfiguration = "Store";

    const string AddInBinPrefix = "AddIn";
    const string ArtifactsFolder = "output";

    const string WixTargetPath = @"%USERPROFILE%\.nuget\packages\wixsharp\1.18.1\build\WixSharp.targets";
    const string IlRepackTargetPath = @"%USERPROFILE%\.nuget\packages\ilrepack.lib.msbuild.task\2.0.18.2\build\ILRepack.Lib.MSBuild.Task.targets";

    static readonly int InstallerCreationTime = IsServerBuild ? 15 : 7;
}