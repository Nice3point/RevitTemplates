using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Build.Options;

/// <summary>
///     Installer configuration options.
/// </summary>
[PublicAPI]
public sealed record InstallerOptions
{
    /// <summary>
    ///     The identity shared by every release of the add-in.
    /// </summary>
    /// <remarks>
    ///     A release published under the installed code upgrades it in place. <br/>
    ///     A release published under a new code installs alongside its predecessor.
    /// </remarks>
    [Required] public Guid? UpgradeCode { get; init; }
}
