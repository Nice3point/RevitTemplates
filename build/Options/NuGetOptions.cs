using JetBrains.Annotations;
using ModularPipelines.Attributes;

namespace Build.Options;

/// <summary>
///     NuGet publishing options.
/// </summary>
[PublicAPI]
public sealed record NuGetOptions
{
    /// <summary>
    ///     API key used to publish packages.
    /// </summary>
    [SecretValue] public string? ApiKey { get; init; }

    /// <summary>
    ///     Package source used for publishing.
    /// </summary>
    public string Source { get; init; } = "https://api.nuget.org/v3/index.json";
}
