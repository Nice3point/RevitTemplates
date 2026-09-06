using JetBrains.Annotations;

namespace Build.Options;

/// <summary>
///     Build output options.
/// </summary>
[PublicAPI]
public sealed record BuildOptions
{
    /// <summary>
    ///     Path to the build output directory.
    /// </summary>
    public string OutputDirectory { get; init; } = "output";
}
