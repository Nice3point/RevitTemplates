namespace Build.Options;

/// <summary>
///     Release publishing options.
/// </summary>
[Serializable]
public sealed record PublishOptions
{
    /// <summary>
    ///     Path to the changelog file.
    /// </summary>
    public string? ChangelogFile { get; init; }
}
