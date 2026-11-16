using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using ModularPipelines.Secrets;

namespace Build.Pipeline.Publishing;

/// <summary>
///     Represents the options of the NuGet publication.
/// </summary>
[PublicAPI]
public sealed class NuGetOptions
{
    /// <summary>
    ///     The name of the configuration section that contains the options.
    /// </summary>
    public const string ConfigurationSectionName = "NuGet";

    /// <summary>
    ///     Gets or sets the API key that authorizes the package push.
    /// </summary>
    [SecretValue]
    public string? ApiKey { get; set; }

    /// <summary>
    ///     Gets or sets the URL of the package source the packages are pushed to.
    /// </summary>
    [Required]
    public string Source { get; set; } = null!;
}
