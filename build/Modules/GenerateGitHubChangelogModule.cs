using System.Text;
using Build.Options;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.GitHub.Extensions;
using ModularPipelines.Modules;

namespace Build.Modules;

[DependsOn<GenerateChangelogModule>]
[DependsOn<ResolveVersioningModule>]
public sealed class GenerateGitHubChangelogModule : Module<string>
{
    protected override async Task<string?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await GetModule<ResolveVersioningModule>();
        var changelogResult = await GetModule<GenerateChangelogModule>();
        var versioning = versioningResult.Value!;
        var changelog = changelogResult.Value!;

        return AppendGitHubCompareUrl(context, changelog, versioning);
    }

    private static string AppendGitHubCompareUrl(IPipelineContext context, string changelog, ResolveVersioningResult versioning)
    {
        if (changelog.Contains("Full changelog", StringComparison.OrdinalIgnoreCase)) return changelog;

        var repositoryInfo = context.GitHub().RepositoryInfo;
        var url = $"https://github.com/{repositoryInfo.Identifier}/compare/{versioning.PreviousVersion}...{versioning.Version}";

        return $"{changelog}{Environment.NewLine}{Environment.NewLine}**Full changelog**: {url}";
    }
}