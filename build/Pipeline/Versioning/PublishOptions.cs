using JetBrains.Annotations;

namespace Build.Pipeline.Versioning;

/// <summary>
///     Represents the options of the release publication.
/// </summary>
[PublicAPI]
public sealed class PublishOptions
{
    /// <summary>
    ///     The name of the configuration section that contains the options.
    /// </summary>
    public const string ConfigurationSectionName = "Publish";

    /// <summary>
    ///     Gets or sets the version the packages are compiled and published under.
    /// </summary>
    /// <remarks>If the value is empty, GitVersion calculates the version from the Git history.</remarks>
    /// <example><c>6.3.0-preview.1.20261007</c> or <c>6.3.0</c>.</example>
    public string? Version { get; set; }
}
