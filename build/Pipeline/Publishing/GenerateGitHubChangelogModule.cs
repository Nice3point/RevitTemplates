using Build.Pipeline.Versioning;
using ModularPipelines.Context;

namespace Build.Pipeline.Publishing;

/// <summary>
///     Represents the pipeline step that formats the release notes for the GitHub release.
/// </summary>
[DependsOn<GenerateChangelogModule>]
[DependsOn<ResolveVersioningModule>]
public sealed class GenerateGitHubChangelogModule : Module<string>
{
    /// <inheritdoc />
    protected override async Task<string> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var changelogResult = await context.GetModule<GenerateChangelogModule>();

        var versioning = versioningResult.Value;
        var changelog = changelogResult.Value;

        return AppendGitHubCompareUrl(context, changelog, versioning);
    }

    /// <summary>
    ///     Appends the link to the comparison with the previous release if the release notes don't contain a full changelog link.
    /// </summary>
    private static string AppendGitHubCompareUrl(IPipelineContext context, string changelog, ResolveVersioningResult versioning)
    {
        if (changelog.Contains("Full changelog", StringComparison.OrdinalIgnoreCase))
        {
            return changelog;
        }

        var repositoryInfo = context.Tools.GitHub.RepositoryInfo;
        var previousRevision = versioning.PreviousRelease switch
        {
            ReleaseTag tag => tag.Name,
            RootCommit commit => commit.Sha
        };

        var url = $"https://github.com/{repositoryInfo.Identifier}/compare/{previousRevision}...{versioning.Version}";

        return $"{changelog}{Environment.NewLine}{Environment.NewLine}**Full changelog**: {url}";
    }
}
