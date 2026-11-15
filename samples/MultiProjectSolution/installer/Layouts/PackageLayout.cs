using Installer.Manifest;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;

namespace Installer.Layouts;

/// <summary>
///     Provides extension methods for <see cref="InstallerManifest" /> to create the package of an installation scope.
/// </summary>
/// <remarks>The SingleUser package installs for the current user, and the MultiUser package installs for all users.</remarks>
public static class PackageLayout
{
    /// <summary>
    ///     Defines extensions for <see cref="InstallerManifest" />.
    /// </summary>
    /// <param name="manifest">The manifest of the packages.</param>
    extension(InstallerManifest manifest)
    {
        /// <summary>
        ///     Creates the <see cref="Project" /> of the package for the specified installation scope.
        /// </summary>
        /// <param name="contentRoot">The root directory of the relative content paths.</param>
        /// <param name="scope">The installation scope of the package.</param>
        /// <returns>The project that builds the <c>&lt;product&gt;-&lt;release&gt;-&lt;scope&gt;.msi</c> package.</returns>
        /// <exception cref="DirectoryNotFoundException">The base directory of a file set doesn't exist.</exception>
        /// <exception cref="InvalidDataException">A file set matches no files.</exception>
        public Project CreatePackageLayout(DirectoryInfo contentRoot, InstallScope scope)
        {
            var packageName = scope switch
            {
                InstallScope.perUser => "SingleUser",
                _ => "MultiUser"
            };

            var mediaLayout = new MediaLayout(manifest);
            var project = new Project
            {
                OutDir = manifest.OutputDirectory,
                OutFileName = $"{manifest.ProductName}-{manifest.ReleaseVersion}-{packageName}",
                Name = manifest.ProductName,
                GUID = manifest.UpgradeCode,
                Version = manifest.ProductVersion,
                Platform = Platform.x64,
                Scope = scope,
                Dirs = manifest.CreateFeatureLayout(contentRoot, scope, mediaLayout),
                Media = mediaLayout.CreateMedia(),
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

            return project;
        }
    }
}
