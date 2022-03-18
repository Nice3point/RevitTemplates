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
}