using JetBrains.Annotations;

namespace Build.Options;

/// <summary>
///     Release version options.
/// </summary>
[PublicAPI]
public sealed record PublishOptions
{
    /// <summary>
    ///     Explicit release version, or automatic versioning when absent.
    /// </summary>
    public string? Version { get; init; }
}
