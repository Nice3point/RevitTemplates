using JetBrains.Annotations;

namespace Build.Options;

/// <summary>
///     Represents the options of the release publication.
/// </summary>
[PublicAPI]
public sealed record PublishOptions
{
    /// <summary>
    ///     The name of the configuration section that contains the options.
    /// </summary>
    public const string ConfigurationSectionName = "Publish";

    /// <summary>
    ///     Gets the path to the changelog file that contains the release notes.
    /// </summary>
    /// <value>A path relative to the repository root.</value>
    /// <remarks>If the value is empty, the file doesn't exist, or the file has no entry for the version, GitHub generates the release notes.</remarks>
    public string? ChangelogFile { get; init; }
}
