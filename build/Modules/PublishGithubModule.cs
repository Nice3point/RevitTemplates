using Build.Options;
using EnumerableAsyncProcessor.Extensions;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Git.Options;
using ModularPipelines.GitHub.Attributes;
using ModularPipelines.GitHub.Extensions;
using ModularPipelines.Modules;
using Octokit;
using Shouldly;
using Status = ModularPipelines.Enums.Status;

namespace Build.Modules;

[SkipIfNoGitHubToken]
[DependsOn<PackTemplatesModule>]
[DependsOn<GenerateGitHubChangelogModule>]
public sealed class PublishGithubModule(IOptions<BuildOptions> buildOptions) : Module<ReleaseAsset[]?>
{
    protected override async Task<ReleaseAsset[]?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var changelog = await GetModule<GenerateGitHubChangelogModule>();
        var outputFolder = context.Git().RootDirectory.GetFolder(buildOptions.Value.OutputDirectory);
        var targetFiles = outputFolder.ListFiles().ToArray();
        targetFiles.ShouldNotBeEmpty("No artifacts were found to create the Release");

        var repositoryInfo = context.GitHub().RepositoryInfo;
        var newRelease = new NewRelease(buildOptions.Value.Version)
        {
            Name = buildOptions.Value.Version,
            Body = changelog.Value,
            TargetCommitish = context.Git().Information.LastCommitSha,
            Prerelease = buildOptions.Value.Version.Contains('-')
        };

        var release = await context.GitHub().Client.Repository.Release.Create(repositoryInfo.Owner, repositoryInfo.RepositoryName, newRelease);
        return await targetFiles
            .SelectAsync(async file =>
            {
                var asset = new ReleaseAssetUpload
                {
                    ContentType = "application/x-binary",
                    FileName = file.Name,
                    RawData = file.GetStream()
                };
                return await context.GitHub().Client.Repository.Release.UploadAsset(release, asset, cancellationToken);
            }, cancellationToken)
            .ProcessOneAtATime();
    }

    protected override async Task OnAfterExecute(IPipelineContext context)
    {
        if (Status == Status.Failed)
        {
            await context.Git().Commands.Push(new GitPushOptions
            {
                Delete = true,
                Arguments = ["origin", buildOptions.Value.Version]
            });
        }
    }
}