using Installer;
using Installer.Layouts;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;

var manifestFile = new FileInfo(args[0]);
var manifest = manifestFile.ReadManifest();
var contentRoot = manifestFile.Directory!;
var installOrder = new MediaLayout(manifest.Content);

var project = new Project
{
    OutDir = manifest.OutputDirectory,
    Name = manifest.ProductName,
    GUID = manifest.UpgradeCode,
    Version = manifest.ProductVersion,
    Platform = Platform.x64,
    UI = WUI.WixUI_FeatureTree,
    MajorUpgrade = MajorUpgrade.Default,
    BannerImage = @"install\Resources\Icons\BannerImage.png",
    BackgroundImage = @"install\Resources\Icons\BackgroundImage.png",
    ControlPanelInfo =
    {
        Manufacturer = Environment.UserName,
        ProductIcon = @"install\Resources\Icons\ShellIcon.ico"
    }
};

project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.CustomizeDlg);
project.WixSourceGenerated += installOrder.WriteToWixSource;

BuildSingleUserMsi();
BuildMultiUserMsi();
return;

void BuildSingleUserMsi()
{
    project.Scope = InstallScope.perUser;
    project.OutFileName = $"{manifest.ProductName}-{manifest.ReleaseVersion}-SingleUser";
    project.Dirs = manifest.Content.CreateFeatureLayout(contentRoot, InstallScope.perUser, installOrder);
    project.BuildMsi();
}

void BuildMultiUserMsi()
{
    project.Scope = InstallScope.perMachine;
    project.OutFileName = $"{manifest.ProductName}-{manifest.ReleaseVersion}-MultiUser";
    project.Dirs = manifest.Content.CreateFeatureLayout(contentRoot, InstallScope.perMachine, installOrder);
    project.BuildMsi();
}
