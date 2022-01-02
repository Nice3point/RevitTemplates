partial class Build
{
    readonly string[] Projects =
    {
        "Nice3point.Revit.Solution"
    };

<!--#if (Installer)
    public const string InstallerProject = "Installer";

#endif-->
    public const string BuildConfiguration = "Release";
<!--#if (Installer)
    public const string InstallerConfiguration = "Installer";
#endif-->
<!--#if (Bundle)
    public const string BundleConfiguration = "";
#endif-->

    const string AddInBinPrefix = "AddIn";
    const string ArtifactsFolder = "output";

    //Specify the path to the MSBuild.exe file here if you are not using VisualStudio
    const string CustomMsBuildPath = @"C:\Program Files\JetBrains\JetBrains Rider\tools\MSBuild\Current\Bin\MSBuild.exe";
}