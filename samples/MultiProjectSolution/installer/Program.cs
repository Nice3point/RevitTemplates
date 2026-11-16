using Installer.Layouts;
using Installer.Manifest;
using WixSharp;

if (args is not [var manifestPath])
{
    throw new ArgumentException("Cannot create the installer. The path of the installer manifest is not specified.");
}

var manifestFile = new FileInfo(manifestPath);
var manifest = manifestFile.ReadJson(InstallerJsonContext.Default.InstallerManifest);
var contentRoot = manifestFile.Directory!;

InstallScope[] scopes =
[
    InstallScope.perUser,
    InstallScope.perMachine
];

foreach (var scope in scopes)
{
    var package = manifest.CreatePackageLayout(contentRoot, scope);
    package.BuildMsi();
}
