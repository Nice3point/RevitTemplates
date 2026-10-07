using Installer;
using Installer.Layouts;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;

if (args is not [var manifestPath])
{
    throw new ArgumentException("Cannot create the installer. The path of the installer manifest is not specified.");
}

var manifestFile = new FileInfo(manifestPath);
var manifest = manifestFile.ReadManifest();
var contentRoot = manifestFile.Directory!;
var mediaLayout = new MediaLayout(manifest.Content);

var project = new Project
{
    OutDir = manifest.OutputDirectory,
    Name = manifest.ProductName,
    GUID = manifest.UpgradeCode,
    Version = manifest.ProductVersion,
    Platform = Platform.x64,
    UI = WUI.WixUI_FeatureTree,
    MajorUpgrade = MajorUpgrade.Default,
    BannerImage = @"installer\Resources\Icons\BannerImage.png",
    BackgroundImage = @"installer\Resources\Icons\BackgroundImage.png",
    ControlPanelInfo =
    {
        Manufacturer = Environment.UserName,
        ProductIcon = @"installer\Resources\Icons\ShellIcon.ico"
    }
};

project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.CustomizeDlg);
project.WixSourceGenerated += mediaLayout.WriteToWixSource;

BuildSingleUserMsi();
BuildMultiUserMsi();
return;

void BuildSingleUserMsi()
{
    project.Scope = InstallScope.perUser;
    project.OutFileName = $"{manifest.ProductName}-{manifest.ReleaseVersion}-SingleUser";
    project.Dirs = manifest.Content.CreateFeatureLayout(contentRoot, InstallScope.perUser, mediaLayout);
    project.BuildMsi();
}

void BuildMultiUserMsi()
{
    project.Scope = InstallScope.perMachine;
    project.OutFileName = $"{manifest.ProductName}-{manifest.ReleaseVersion}-MultiUser";
    project.Dirs = manifest.Content.CreateFeatureLayout(contentRoot, InstallScope.perMachine, mediaLayout);
    project.BuildMsi();
}
