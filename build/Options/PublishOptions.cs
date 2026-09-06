namespace Build.Options;

/// <summary>
///     Release version options.
/// </summary>
[Serializable]
public sealed record PublishOptions
{
    /// <summary>
    ///     Explicit release version, or automatic versioning when absent.
    /// </summary>
    public string? Version { get; init; }
}
