using JetBrains.Annotations;

namespace Installer.Manifest;

/// <summary>
///     Provides the roles of the file sets the build pipeline writes to the installer manifest.
/// </summary>
/// <remarks>The role selects the cabinet of the files and the order in which Windows Installer installs them.</remarks>
[PublicAPI]
public static class FileSetRoles
{
    /// <summary>
    ///     The assemblies and the resources of the add-in for one Revit version.
    /// </summary>
    public const string Payload = "payload";

    /// <summary>
    ///     The add-in manifest Revit loads, installed after the payload it refers to.
    /// </summary>
    public const string AddinManifest = "addin";
}
