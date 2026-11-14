using System.Text;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.GitHub.Extensions;
using ModularPipelines.Modules;

namespace Build.Modules;

/// <summary>
///     Represents the pipeline step that formats the release notes for the GitHub release.
/// </summary>
[DependsOn<GenerateChangelogModule>]
[DependsOn<ResolveVersioningModule>]
public sealed class GenerateGitHubChangelogModule : Module<string>
{
    protected override async Task<string?> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var changelogResult = await context.GetModule<GenerateChangelogModule>();
        var versioning = versioningResult.ValueOrDefault!;
        var changelog = changelogResult.ValueOrDefault!;

        return AppendGitHubCompareUrl(context, changelog, versioning);
    }

    /// <summary>
    ///     Appends the link to the comparison with the previous release if the release notes don't contain a full changelog link.
    /// </summary>
    private static string AppendGitHubCompareUrl(IModuleContext context, string changelog, ResolveVersioningResult versioning)
    {
        var repositoryInfo = context.GitHub().RepositoryInfo;
        StringBuilder? changelogBuilder = null;

        if (!changelog.Contains("Full changelog", StringComparison.OrdinalIgnoreCase))
        {
            changelogBuilder ??= new StringBuilder(changelog);
            changelogBuilder.AppendLine()
                .AppendLine()
                .Append("**Full changelog**: ")
                .AppendLine($"https://github.com/{repositoryInfo.Identifier}/compare/{versioning.PreviousVersion}...{versioning.Version}");
        }

        return changelogBuilder?.ToString() ?? changelog;
    }
}
