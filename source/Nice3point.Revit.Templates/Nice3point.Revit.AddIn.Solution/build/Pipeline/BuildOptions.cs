#if (hasArtifacts)
using System.ComponentModel.DataAnnotations;
#endif
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
    ///     Gets or sets the version the add-in is compiled and published under.
    /// </summary>
    /// <remarks>If the value is empty, GitVersion calculates the version from the Git history.</remarks>
    /// <example><c>1.0.0-alpha.1.250101</c>, <c>1.0.0-beta.2.250101</c>, or <c>1.0.0</c>.</example>
    public string? Version { get; set; }
#if (hasArtifacts)

    /// <summary>
    ///     Gets or sets the path to the output directory of the build artifacts.
    /// </summary>
    /// <value>A path relative to the repository root.</value>
    [Required]
    public string OutputDirectory { get; set; } = null!;
#endif
}
