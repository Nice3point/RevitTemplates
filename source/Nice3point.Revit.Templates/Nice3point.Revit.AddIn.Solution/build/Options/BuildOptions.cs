#if (hasArtifacts)
using System.ComponentModel.DataAnnotations;

#endif
namespace Build.Options;

/// <summary>
///     Build configuration options.
/// </summary>
[Serializable]
public sealed record BuildOptions
{
    /// <summary>
    ///     Application version
    /// </summary>
    /// <example>
    ///     1.0.0-alpha.1.250101 <br/>
    ///     1.0.0-beta.2.250101 <br/>
    ///     1.0.0
    /// </example>
    public string? Version { get; init; }
#if (hasArtifacts)

    /// <summary>
    ///     Path to build output
    /// </summary>
    [Required] public string OutputDirectory { get; init; } = null!;
#endif
}