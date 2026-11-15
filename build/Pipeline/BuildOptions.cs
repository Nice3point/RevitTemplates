using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Build.Pipeline;

/// <summary>
///     Represents the options that the steps of the build pipeline share.
/// </summary>
[PublicAPI]
public sealed class BuildOptions
{
    /// <summary>
    ///     The name of the configuration section that contains the options.
    /// </summary>
    public const string ConfigurationSectionName = "Build";

    /// <summary>
    ///     Gets or sets the path to the output directory of the build artifacts.
    /// </summary>
    /// <value>A path relative to the repository root.</value>
    [Required]
    public string OutputDirectory { get; set; } = null!;
}
