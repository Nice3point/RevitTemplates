using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Build.Options;

/// <summary>
///     Represents the options of the build pipeline.
/// </summary>
[PublicAPI]
public sealed record BuildOptions
{
    /// <summary>
    ///     The name of the configuration section that contains the options.
    /// </summary>
    public const string ConfigurationSectionName = "Build";

    /// <summary>
    ///     Gets the path to the output directory of the build artifacts.
    /// </summary>
    /// <value>A path relative to the repository root.</value>
    [Required]
    public string OutputDirectory { get; init; } = null!;
}
