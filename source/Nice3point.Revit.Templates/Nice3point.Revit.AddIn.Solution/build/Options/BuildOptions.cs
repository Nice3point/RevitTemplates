#if (hasArtifacts)
using System.ComponentModel.DataAnnotations;
#endif
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
    ///     Gets the version the add-in is compiled and published under.
    /// </summary>
    /// <remarks>If the value is empty, GitVersion calculates the version from the Git history.</remarks>
    /// <example>
    ///     1.0.0-alpha.1.250101 <br />
    ///     1.0.0-beta.2.250101 <br />
    ///     1.0.0
    /// </example>
    public string? Version { get; init; }
#if (hasArtifacts)

    /// <summary>
    ///     Gets the path to the output directory of the build artifacts.
    /// </summary>
    /// <value>A path relative to the repository root.</value>
    [Required]
    public string OutputDirectory { get; init; } = null!;
#endif
}
