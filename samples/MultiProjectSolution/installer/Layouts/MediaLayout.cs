using Installer.Manifest;
using WixSharp;

namespace Installer.Layouts;

/// <summary>
///     Represents the cabinet files of the installer packages, one cabinet per file set role.
/// </summary>
/// <remarks>
///     Windows Installer copies the files of the cabinets in the order of their disk IDs.
///     The roles are installed in the order in which they appear in the manifest.
/// </remarks>
public sealed class MediaLayout
{
    private readonly string[] _roles;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MediaLayout" /> class.
    /// </summary>
    /// <param name="manifest">The manifest whose file sets define the roles.</param>
    public MediaLayout(InstallerManifest manifest)
    {
        _roles =
        [
            .. manifest.Content
                .SelectMany(addin => addin.Files)
                .Select(fileSet => fileSet.Role)
                .Distinct(StringComparer.OrdinalIgnoreCase)
        ];
    }

    /// <summary>
    ///     Gets the disk ID of the cabinet that contains the specified file set role.
    /// </summary>
    /// <param name="role">The role of the file set.</param>
    /// <returns>The one-based disk ID of the cabinet.</returns>
    /// <exception cref="InvalidDataException">The manifest has no file set with the specified role.</exception>
    public int ResolveDiskId(string role)
    {
        var index = Array.FindIndex(_roles, candidate => string.Equals(candidate, role, StringComparison.OrdinalIgnoreCase));
        if (index < 0)
        {
            throw new InvalidDataException($"Cannot resolve the cabinet of the '{role}' file set. The manifest has no file set with this role.");
        }

        return index + 1;
    }

    /// <summary>
    ///     Creates the embedded cabinets of the package, one per file set role.
    /// </summary>
    /// <returns>The media of the package, in the order of their disk IDs.</returns>
    public List<Media> CreateMedia()
    {
        return
        [
            .. _roles.Select(static (role, index) => new Media
            {
                Id = index + 1,
                Cabinet = $"{role}.cab",
                EmbedCab = true
            })
        ];
    }
}
