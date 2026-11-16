using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.Git.Options;
using ModularPipelines.Options;

namespace Build.Pipeline.Versioning;

/// <summary>
///     Represents the pipeline step that resolves the version the add-in is compiled and published under.
/// </summary>
/// <remarks>If <see cref="BuildOptions.Version" /> is set, the step uses that value; otherwise, GitVersion calculates the version from the Git history.</remarks>
public sealed class ResolveVersioningModule(IOptions<BuildOptions> buildOptions) : Module<ResolveVersioningResult>
{
    /// <inheritdoc />
    protected override async Task<ResolveVersioningResult> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var version = buildOptions.Value.Version;
        var versioning = string.IsNullOrEmpty(version) switch
        {
            true => await CreateFromGitVersioningAsync(context, cancellationToken),
            false => await CreateFromVersionStringAsync(context, version, cancellationToken)
        };

        context.Summary.KeyValue("Build", "Version", versioning.Version);
        return versioning;
    }

    /// <summary>
    ///     Resolves the versions from the specified version string.
    /// </summary>
    private static async Task<ResolveVersioningResult> CreateFromVersionStringAsync(IModuleContext context, string version, CancellationToken cancellationToken)
    {
        var versionParts = version.Split(separator: '-', count: 2);

        return new ResolveVersioningResult
        {
            Version = version,
            VersionPrefix = versionParts[0],
            VersionSuffix = versionParts.Length > 1 ? versionParts[1] : null,
            IsPrerelease = versionParts.Length > 1,
            PreviousRelease = await FetchPreviousReleaseAsync(context, cancellationToken)
        };
    }

    /// <summary>
    ///     Resolves the versions from the Git history by using GitVersion.
    /// </summary>
    private static async Task<ResolveVersioningResult> CreateFromGitVersioningAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var gitVersioning = await context.Tools.Git.Versioning.GetVersioningInformationAsync(cancellationToken);

        return new ResolveVersioningResult
        {
            Version = gitVersioning.SemVer!,
            VersionPrefix = gitVersioning.MajorMinorPatch!,
            VersionSuffix = gitVersioning.PreReleaseTag,
            IsPrerelease = !string.IsNullOrEmpty(gitVersioning.PreReleaseLabel),
            PreviousRelease = await FetchPreviousReleaseAsync(context, cancellationToken)
        };
    }

    /// <summary>
    ///     Gets the tag of the previous release, or the root commit if no earlier commit has a tag.
    /// </summary>
    private static async Task<PreviousRelease> FetchPreviousReleaseAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var describeResult = await context.Tools.Git.Commands.History.DescribeAsync(
            new GitDescribeOptions
            {
                Tags = true,
                Abbrev = "0",
                Arguments = ["HEAD^"]
            },
            new CommandExecutionOptions
            {
                ThrowOnNonZeroExitCode = false,
                Logging = CommandLoggingOptions.Silent
            }, cancellationToken);

        var previousTag = describeResult.StandardOutput.Trim();
        if (!string.IsNullOrWhiteSpace(previousTag))
        {
            return new ReleaseTag(previousTag);
        }

        var revisionResult = await context.Tools.Git.Commands.History.RevListAsync(
            new GitRevListOptions
            {
                MaxParents = "0",
                MaxCount = "1",
                Pretty = "format:%H",
                NoCommitHeader = true,
                Ref = "HEAD"
            },
            new CommandExecutionOptions
            {
                Logging = CommandLoggingOptions.Silent
            }, cancellationToken);

        return new RootCommit(revisionResult.StandardOutput.Trim());
    }
}
