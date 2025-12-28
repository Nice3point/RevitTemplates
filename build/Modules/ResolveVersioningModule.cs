using Build.Options;
using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.Enums;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Git.Options;
using ModularPipelines.Modules;
using Shouldly;

namespace Build.Modules;

public sealed class ResolveVersioningModule(IOptions<BuildOptions> buildOptions) : Module<ResolveVersioningResult>
{
    protected override async Task<ResolveVersioningResult?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var version = buildOptions.Value.Version;
        if (context.Environment.EnvironmentName == "Production")
        {
            version.ShouldNotBeNullOrWhiteSpace();
        }

        return await CreateFromVersionStringAsync(context, version!);
    }

    private static async Task<ResolveVersioningResult> CreateFromVersionStringAsync(IPipelineContext context, string version)
    {
        var versionParts = version.Split('-');

        return new ResolveVersioningResult
        {
            Version = version,
            VersionPrefix = versionParts[0],
            VersionSuffix = versionParts.Length > 1 ? versionParts[1] : null,
            IsPrerelease = versionParts.Length > 1,
            PreviousVersion = await FetchPreviousVersionAsync(context)
        };
    }

    private static async Task<string> FetchPreviousVersionAsync(IPipelineContext context)
    {
        var describeResult = await context.Git().Commands.Describe(new GitDescribeOptions
        {
            Tags = true,
            Abbrev = "0",
            Arguments = ["HEAD^"],
            ThrowOnNonZeroExitCode = false,
            CommandLogging = CommandLogging.None
        });

        var previousTag = describeResult.StandardOutput.Trim();
        if (!string.IsNullOrWhiteSpace(previousTag)) return previousTag;

        var revisionResult = await context.Git().Commands.RevList(new GitRevListOptions
        {
            MaxParents = "0",
            MaxCount = "1",
            Pretty = "format:%H",
            Arguments = ["HEAD"],
            NoCommitHeader = true,
            CommandLogging = CommandLogging.None
        });

        return revisionResult.StandardOutput.Trim();
    }
}

public sealed record ResolveVersioningResult
{
    /// <summary>
    ///     Release version, includes version number and release stage.
    /// </summary>
    /// <remarks>Version format: <c>version-environment.n.date</c>.</remarks>
    /// <example>
    ///     1.0.0-alpha.1.250101 <br/>
    ///     1.0.0-beta.2.250101 <br/>
    ///     1.0.0
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
    public required string VersionPrefix { get; init; }

    /// <summary>
    ///     The pre-release label of the release version number.
    /// </summary>
    /// <example>
    ///     alpha <br/>
    ///     beta <br/>
    ///     rc.1.250101
    /// </example>
    public required string? VersionSuffix { get; init; }

    /// <summary>
    ///     Indicates whether the current version represents a prerelease.
    /// </summary>
    /// <remarks>
    /// A version is considered a prerelease if it includes a version suffix,
    /// such as "alpha", "beta", or similar identifiers.
    /// </remarks>
    public required bool IsPrerelease { get; init; }

    /// <summary>
    ///     The previous release version.
    /// </summary>
    public required string PreviousVersion { get; init; }
}