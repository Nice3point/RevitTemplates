#if (hasArtifacts)
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

#endif
namespace Build.Options;

/// <summary>
///     Build configuration options.
/// </summary>
[PublicAPI]
public sealed record BuildOptions
{
    /// <summary>
    ///     Application version.
    /// </summary>
    /// <remarks>
    ///     The configured value overrides the version determined by GitVersion.Tool.
    /// </remarks>
    /// <example>
    ///     1.0.0-alpha.1.250101 <br/>
    ///     1.0.0-beta.2.250101 <br/>
    ///     1.0.0
    /// </example>
    public string? Version { get; init; }
#if (hasArtifacts)

    /// <summary>
    ///     Path to the build output directory.
    /// </summary>
    [Required] public string OutputDirectory { get; init; } = null!;
#endif
}
