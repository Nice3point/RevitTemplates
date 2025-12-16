using Build.Options;
using EnumerableAsyncProcessor.Extensions;
using Microsoft.Extensions.Logging;
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

/// <summary>
///     Publish the add-in to GitHub.
/// </summary>
/// <param name="buildOptions"></param>
[SkipIfNoGitHubToken]
[DependsOn<ResolveVersioningModule>]
[DependsOn<GenerateGitHubChangelogModule>]
#if (bundle)
[DependsOn<CreateBundleModule>]
#endif
#if (installer)
[DependsOn<CreateInstallersModule>]
#endif
public sealed class PublishGithubModule(IOptions<BuildOptions> buildOptions) : Module<ReleaseAsset[]?>
{
    protected override async Task<ReleaseAsset[]?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await GetModule<ResolveVersioningModule>();
        var changelogResult = await GetModule<GenerateGitHubChangelogModule>();
        var versioning = versioningResult.Value!;
        var changelog = changelogResult.Value!;

        var outputFolder = context.Git().RootDirectory.GetFolder(buildOptions.Value.OutputDirectory);
        var targetFiles = outputFolder.ListFiles().ToArray();
        targetFiles.ShouldNotBeEmpty("No artifacts were found to create the Release");

        var repositoryInfo = context.GitHub().RepositoryInfo;
        var newRelease = new NewRelease(versioning.Version)
        {
            Name = versioning.Version,
            Body = changelog,
            TargetCommitish = context.Git().Information.LastCommitSha,
            Prerelease = versioning.IsPrerelease
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

                context.Logger.LogInformation("Uploading asset: {Asset}", asset.FileName);

                return await context.GitHub().Client.Repository.Release.UploadAsset(release, asset, cancellationToken);
            }, cancellationToken)
            .ProcessInParallel();
    }

    protected override async Task OnAfterExecute(IPipelineContext context)
    {
        if (Status == Status.Failed)
        {
            var buildVersioning = await GetModule<ResolveVersioningModule>();
            await context.Git().Commands.Push(new GitPushOptions
            {
                Delete = true,
                Arguments = ["origin", buildVersioning.Value!.Version]
            });
        }
    }
}