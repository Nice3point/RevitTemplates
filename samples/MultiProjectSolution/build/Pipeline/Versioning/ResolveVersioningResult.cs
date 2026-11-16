using JetBrains.Annotations;

namespace Build.Pipeline.Versioning;

/// <summary>
///     Represents the versions the add-in is compiled and published under.
/// </summary>
[PublicAPI]
public sealed record ResolveVersioningResult
{
    /// <summary>
    ///     Gets or initializes the release version with the optional prerelease label.
    /// </summary>
    /// <example><c>1.0.0-alpha.1</c>, <c>12.3.6-rc.2.250101</c>, or <c>2026.4.0</c>.</example>
    public required string Version { get; init; }

    /// <summary>
    ///     Gets or initializes the major, minor, and patch components of the release version.
    /// </summary>
    /// <example><c>1.0.0</c>, <c>12.3.6</c>, or <c>2026.4.0</c>.</example>
    public required string VersionPrefix { get; init; }

    /// <summary>
    ///     Gets or initializes the prerelease label of the release version, or <see langword="null" /> for a stable release.
    /// </summary>
    /// <example><c>alpha.1</c>, <c>beta</c>, or <c>rc.2.250101</c>.</example>
    public required string? VersionSuffix { get; init; }

    /// <summary>
    ///     Gets or initializes a value indicating whether the release version has a prerelease label.
    /// </summary>
    public required bool IsPrerelease { get; init; }

    /// <summary>
    ///     Gets or initializes the release the version is compared with: the tag of the previous release, or the root commit if no earlier commit has a tag.
    /// </summary>
    public required PreviousRelease PreviousRelease { get; init; }
}
