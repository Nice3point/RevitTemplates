using System.Text;
using Build.Options;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.GitHub.Extensions;
using ModularPipelines.Modules;

namespace Build.Modules;

[DependsOn<CreateChangelogModule>]
public sealed class CreateGitHubChangelogModule(IOptions<PackOptions> packOptions) : Module<string>
{
    protected override async Task<string?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var changelogModule = await GetModule<CreateChangelogModule>();

        var changelog = await AppendGitHubCompareUrlAsync(context, changelogModule.Value!);
        return changelog.ToString();
    }

    private async Task<StringBuilder> AppendGitHubCompareUrlAsync(IPipelineContext context, StringBuilder changelog)
    {
        var repositoryInfo = context.GitHub().RepositoryInfo;
        var repositoryId = long.Parse(context.GitHub().EnvironmentVariables.RepositoryId!);
        var latestRelease = await context.GitHub().Client.Repository.Release.GetLatest(repositoryId);

        if (changelog[^1] != '\r' || changelog[^1] != '\n') changelog.AppendLine(Environment.NewLine);
        changelog.Append("Full changelog: ");
        changelog.Append($"https://github.com/{repositoryInfo.Identifier}/compare/{latestRelease.TagName}...{packOptions.Value.Version}");

        return changelog;
    }
}