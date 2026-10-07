using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using ModularPipelines.Attributes;

namespace Build.Options;

/// <summary>
///     Represents the options of the NuGet publication.
/// </summary>
[PublicAPI]
public sealed record NuGetOptions
{
    /// <summary>
    ///     The name of the configuration section that contains the options.
    /// </summary>
    public const string ConfigurationSectionName = "NuGet";

    /// <summary>
    ///     Gets the API key that authorizes the package push.
    /// </summary>
    [SecretValue]
    public string? ApiKey { get; init; }

    /// <summary>
    ///     Gets the URL of the package source the packages are pushed to.
    /// </summary>
    [Required]
    public string Source { get; init; } = null!;
}
