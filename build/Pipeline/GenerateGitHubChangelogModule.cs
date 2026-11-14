using ModularPipelines.Context;

namespace Build.Pipeline;

/// <summary>
///     Represents the pipeline step that formats the release notes for the GitHub release.
/// </summary>
[DependsOn<GenerateChangelogModule>]
[DependsOn<ResolveVersioningModule>]
public sealed class GenerateGitHubChangelogModule : Module<string>
{
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
        var url = $"https://github.com/{repositoryInfo.Identifier}/compare/{versioning.PreviousVersion}...{versioning.Version}";

        return $"{changelog}{Environment.NewLine}{Environment.NewLine}**Full changelog**: {url}";
    }
}
