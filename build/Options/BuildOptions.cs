namespace Build.Options;

/// <summary>
///     Build output options.
/// </summary>
[Serializable]
public sealed record BuildOptions
{
    /// <summary>
    ///     Path to the build output directory.
    /// </summary>
    public string OutputDirectory { get; init; } = "output";
}
