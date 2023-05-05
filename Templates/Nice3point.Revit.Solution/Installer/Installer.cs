using System;
using Installer;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;

const string version = "1.0.0";
const string outputName = "Nice3point.Revit.Solution";
const string projectName = "Nice3point.Revit.Solution";

var project = new Project
{
    Name = projectName,
    OutDir = "output",
    Platform = Platform.x64,
    UI = WUI.WixUI_InstallDir,
    Version = new Version(version),
    MajorUpgrade = MajorUpgrade.Default,
    GUID = new Guid("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD"),
    BannerImage = @"Installer\Resources\Icons\BannerImage.png",
    BackgroundImage = @"Installer\Resources\Icons\BackgroundImage.png",
    ControlPanelInfo =
    {
        Manufacturer = Environment.UserName,
        ProductIcon = @"Installer\Resources\Icons\ShellIcon.ico"
    }
};

var wixEntities = Generator.GenerateWixEntities(args);
project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);

BuildSingleUserMsi();
BuildMultiUserUserMsi();

void BuildSingleUserMsi()
{
    project.InstallScope = InstallScope.perUser;
    project.OutFileName = $"{outputName}-{version}-SingleUser";
    project.Dirs = new Dir[]
    {
        new InstallDir(@"%AppDataFolder%\Autodesk\Revit\Addins\", wixEntities)
    };
    project.BuildMsi();
}

void BuildMultiUserUserMsi()
{
    project.InstallScope = InstallScope.perMachine;
    project.OutFileName = $"{outputName}-{version}-MultiUser";
    project.Dirs = new Dir[]
    {
        new InstallDir(@"%CommonAppDataFolder%\Autodesk\Revit\Addins\", wixEntities)
    };
    project.BuildMsi();
}