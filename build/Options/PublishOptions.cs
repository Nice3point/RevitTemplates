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
    ///     Gets the version the packages are compiled and published under.
    /// </summary>
    /// <remarks>If the value is empty, GitVersion calculates the version from the Git history.</remarks>
    /// <example>
    ///     6.3.0-preview.1.20261007 <br />
    ///     6.3.0
    /// </example>
    public string? Version { get; init; }
}
