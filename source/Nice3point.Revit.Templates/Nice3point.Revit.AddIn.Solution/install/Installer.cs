using Installer;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;

const string outputName = "Nice3point.Revit.AddIn.1";
const string projectName = "Nice3point.Revit.AddIn.1";

var versioning = Versioning.CreateFromVersionString(args[0]);
var project = new Project
{
    OutDir = "output",
    Name = projectName,
    Platform = Platform.x64,
    UI = WUI.WixUI_FeatureTree,
    MajorUpgrade = MajorUpgrade.Default,
    GUID = new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"),
    BannerImage = @"install\Resources\Icons\BannerImage.png",
    BackgroundImage = @"install\Resources\Icons\BackgroundImage.png",
    Version = versioning.VersionPrefix,
    ControlPanelInfo =
    {
        Manufacturer = Environment.UserName,
        ProductIcon = @"install\Resources\Icons\ShellIcon.ico"
    }
};

var wixEntities = Generator.GenerateWixEntities(args[1..]);
project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.CustomizeDlg);

BuildSingleUserMsi();
BuildMultiUserMsi();

void BuildSingleUserMsi()
{
    project.Scope = InstallScope.perUser;
    project.OutFileName = $"{outputName}-{versioning.Version}-SingleUser";
    project.Dirs =
    [
        new InstallDir(@"%AppDataFolder%\Autodesk\Revit\Addins\", wixEntities.Select(entity => entity.Directory).Cast<WixEntity>().ToArray())
    ];
    project.BuildMsi();
}

void BuildMultiUserMsi()
{
    project.Scope = InstallScope.perMachine;
    project.OutFileName = $"{outputName}-{versioning.Version}-MultiUser";

    project.Dirs = wixEntities
        .GroupBy(entity => entity.Version switch
        {
            >= 2027 => @"%ProgramFiles%\Autodesk\Revit\Addins",
            _ => @"%CommonAppDataFolder%\Autodesk\Revit\Addins"
        })
        .Select(root => new InstallDir(root.Key, root.Select(entity => entity.Directory).Cast<WixEntity>().ToArray()))
        .ToArray();

    project.BuildMsi();
}