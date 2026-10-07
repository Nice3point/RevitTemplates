using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Build.Options;

/// <summary>
///     Represents the options of the installer packages.
/// </summary>
[PublicAPI]
public sealed record InstallerOptions
{
    /// <summary>
    ///     The name of the configuration section that contains the options.
    /// </summary>
    public const string ConfigurationSectionName = "Installer";

    /// <summary>
    ///     Gets the upgrade code that identifies all releases of the add-in.
    /// </summary>
    /// <remarks>
    ///     If a release has the same upgrade code as the installed product, the release upgrades the installed product.
    ///     If the upgrade code differs, the release is installed side by side with the previous release.
    /// </remarks>
    [Required]
    public Guid? UpgradeCode { get; init; }
}
