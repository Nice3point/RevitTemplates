using JetBrains.Annotations;

namespace Build.Pipeline.Versioning;

/// <summary>
///     Represents the versions the packages are compiled and published under.
/// </summary>
[PublicAPI]
public sealed record ResolveVersioningResult
{
    /// <summary>
    ///     Gets or initializes the release version with the optional prerelease label.
    /// </summary>
    /// <example><c>6.3.0-preview.1.20261007</c> or <c>6.3.0</c>.</example>
    public required string Version { get; init; }

    /// <summary>
    ///     Gets or initializes the major, minor, and patch components of the release version.
    /// </summary>
    /// <example><c>6.3.0</c>.</example>
    public required string VersionPrefix { get; init; }

    /// <summary>
    ///     Gets or initializes the prerelease label of the release version, or <see langword="null" /> for a stable release.
    /// </summary>
    /// <example><c>preview.1.20261007</c>.</example>
    public required string? VersionSuffix { get; init; }

    /// <summary>
    ///     Gets or initializes a value indicating whether the release version has a prerelease label.
    /// </summary>
    public required bool IsPrerelease { get; init; }

    /// <summary>
    ///     Gets or initializes the tag of the previous release, or the SHA of the root commit if no earlier commit has a tag.
    /// </summary>
    /// <example><c>6.2.3</c> or <c>2f5c3e1a9b7d4c6e8f0a1b2c3d4e5f6a7b8c9d0e</c>.</example>
    public required string PreviousVersion { get; init; }
}
