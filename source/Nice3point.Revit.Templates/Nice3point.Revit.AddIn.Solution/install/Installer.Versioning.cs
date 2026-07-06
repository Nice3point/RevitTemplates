namespace Installer;

public static class Versioning
{
    /// <summary>
    ///     Resolve versions using the specified version string.
    /// </summary>
    public static ResolveVersioningResult CreateFromVersionString(string version)
    {
        var versionParts = version.Split('-');
        var semanticVersion = Version.Parse(versionParts[0]);

        return new ResolveVersioningResult
        {
            Version = version,
            VersionPrefix = semanticVersion,
            VersionSuffix = versionParts.Length > 1 ? versionParts[1] : null
        };
    }
}

public sealed record ResolveVersioningResult
{
    /// <summary>
    ///     Release version, includes version number and release stage.
    /// </summary>
    /// <example>
    ///     1.0.0-alpha.1 <br/>
    ///     12.3.6-rc.2.250101 <br/>
    ///     2026.4.0
    /// </example>
    public required string Version { get; init; }

    /// <summary>
    ///     The normal part of the release version number.
    /// </summary>
    /// <example>
    ///     1.0.0 <br/>
    ///     12.3.6 <br/>
    ///     2026.4.0
    /// </example>
    public required Version VersionPrefix { get; init; }

    /// <summary>
    ///     The pre-release label of the release version number.
    /// </summary>
    /// <example>
    ///     alpha <br/>
    ///     beta.1 <br/>
    ///     rc.2.250101
    /// </example>
    public required string? VersionSuffix { get; init; }
}