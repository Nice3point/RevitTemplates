partial class Build
{
    const string MainProjectName = "Nice3point.Revit.Solution";
    const string InstallerProjectName = "Installer";

    public const string BuildConfiguration = "Release";
    public const string InstallerConfiguration = "Installer";
    public const string BundleSuffixConfiguration = "Store";

    const string AddInBinPrefix = "AddIn";
    const string ArtifactsFolder = "output";
    
    //The libraries below use the AfterBuild target
    //Change the version here if it is different from the one specified in the .csproj file
    const string WixTargetPath = @"%USERPROFILE%\.nuget\packages\wixsharp\1.18.1\build\WixSharp.targets";
    const string IlRepackTargetPath = @"%USERPROFILE%\.nuget\packages\ilrepack.lib.msbuild.task\2.0.18.2\build\ILRepack.Lib.MSBuild.Task.targets";

    //The Wix installer is created in a different thread
    //Increase this value if your installer does not have time to build
    static readonly int InstallerCreationTime = IsServerBuild ? 15 : 7;
}