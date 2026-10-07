using JetBrains.Annotations;

namespace Installer;

/// <summary>
///     Represents the content and identity of the installer packages.
/// </summary>
/// <remarks>
///     The manifest contains the release-specific data.
///     The installer project defines the user interface, the target platform, and the directory layout.
/// </remarks>
[PublicAPI]
public sealed record Manifest
{
    /// <summary>
    ///     Gets the name of the product the packages install.
    /// </summary>
    public required string ProductName { get; init; }

    /// <summary>
    ///     Gets the product version that Windows Installer uses to detect upgrades.
    /// </summary>
    /// <remarks>
    ///     Windows Installer compares the major, minor, and build fields and ignores the revision field.
    ///     The <see href="https://learn.microsoft.com/windows/win32/msi/productversion">ProductVersion</see> property of the MSI database stores the value.
    /// </remarks>
    public required Version ProductVersion { get; init; }

    /// <summary>
    ///     Gets the upgrade code that identifies all releases of the product.
    /// </summary>
    /// <remarks>
    ///     If a release has the same upgrade code as the installed product, the release upgrades the installed product.
    ///     If the upgrade code differs, the release is installed side by side with the previous release.
    /// </remarks>
    public required Guid UpgradeCode { get; init; }

    /// <summary>
    ///     Gets the release version.
    /// </summary>
    /// <remarks>The file name of each package contains the release version.</remarks>
    /// <example>
    ///     1.0.0-alpha.1.250101 <br />
    ///     1.0.0-beta.2.250101 <br />
    ///     1.0.0
    /// </example>
    public required string ReleaseVersion { get; init; }

    /// <summary>
    ///     Gets the absolute path to the output directory of the packages.
    /// </summary>
    public required string OutputDirectory { get; init; }

    /// <summary>
    ///     Gets the add-in content to install.
    /// </summary>
    /// <remarks>Each entry describes the files for a single Revit version and is installed as a separate feature.</remarks>
    public required IReadOnlyList<AddinContent> Content { get; init; }

    /// <summary>
    ///     Represents the add-in files installed for a single Revit version.
    /// </summary>
    [PublicAPI]
    public sealed record AddinContent
    {
        /// <summary>
        ///     Gets the target Revit version.
        /// </summary>
        /// <value>The four-digit Revit release year.</value>
        public required int RevitVersion { get; init; }

        /// <summary>
        ///     Gets the file sets installed for the Revit version.
        /// </summary>
        /// <remarks>The file sets are installed in the order in which their roles first appear in the manifest.</remarks>
        public required IReadOnlyList<FileSet> Files { get; init; }
    }

    /// <summary>
    ///     Represents a set of files selected from a source directory.
    /// </summary>
    [PublicAPI]
    public sealed record FileSet
    {
        /// <summary>
        ///     Gets the installation stage of the file set.
        /// </summary>
        /// <remarks>
        ///     File sets with the same role are installed together, in the order in which the roles are declared.
        ///     A file that Revit loads, such as an <c>.addin</c> manifest, belongs to a role declared after the roles of its dependencies.
        /// </remarks>
        public required string Role { get; init; }

        /// <summary>
        ///     Gets the source directory of the glob patterns.
        /// </summary>
        /// <value>A path relative to the directory of the manifest file.</value>
        public required string BasePath { get; init; }

        /// <summary>
        ///     Gets the glob patterns of the files to include.
        /// </summary>
        /// <remarks>The patterns follow the <see href="https://learn.microsoft.com/dotnet/core/extensions/file-globbing">.NET file globbing</see> format.</remarks>
        public required IReadOnlyList<string> Include { get; init; }

        /// <summary>
        ///     Gets the glob patterns of the files to exclude.
        /// </summary>
        /// <value>Defaults to an empty list.</value>
        /// <remarks>The patterns follow the <see href="https://learn.microsoft.com/dotnet/core/extensions/file-globbing">.NET file globbing</see> format.</remarks>
        public IReadOnlyList<string> Exclude { get; init; } = [];
    }
}
