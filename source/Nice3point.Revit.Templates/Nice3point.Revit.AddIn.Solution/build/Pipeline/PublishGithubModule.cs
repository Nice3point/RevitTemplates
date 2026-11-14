#if (hasArtifacts)
using Build.Options;
using EnumerableAsyncProcessor.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
#endif
using ModularPipelines.Context;
using ModularPipelines.Git.Options;
using ModularPipelines.GitHub;
using Octokit;
#if (hasArtifacts)
using Shouldly;
#endif

namespace Build.Pipeline;

/// <summary>
///     Represents the pipeline step that publishes the GitHub release of the add-in.
/// </summary>
/// <remarks>If the publication fails, the step deletes the version tag from the remote repository.</remarks>
[SkipIfNoGitHubToken]
[DependsOn<ResolveVersioningModule>]
[DependsOn<GenerateGitHubChangelogModule>]
#if (includeBundle)
[DependsOn<CreateBundleModule>(Optional = true)]
#endif
#if (includeInstaller)
[DependsOn<CreateInstallerModule>(Optional = true)]
#endif
#if (hasArtifacts)
public sealed partial class PublishGithubModule(IOptions<BuildOptions> buildOptions) : Module
#else
public sealed class PublishGithubModule : Module
#endif
{
    protected override async Task ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var changelogResult = await context.GetModule<GenerateGitHubChangelogModule>();
        var versioning = versioningResult.Value;
        var changelog = changelogResult.Value;
        var repository = await context.Tools.Git.Information.GetRequiredInfoAsync(cancellationToken);
#if (hasArtifacts)

        var outputFolder = repository.Root.GetFolder(buildOptions.Value.OutputDirectory);
        var targetFiles = outputFolder.ListFiles().ToArray();
        targetFiles.ShouldNotBeEmpty($"Cannot publish the release. No artifacts were found in '{outputFolder.Path}'. Run the build with the 'pack' argument before publishing.");
#endif

        var repositoryInfo = context.Tools.GitHub.RepositoryInfo;
        var newRelease = new NewRelease(versioning.Version)
        {
            Name = versioning.Version,
            Body = changelog,
            TargetCommitish = repository.LastCommitSha,
            Prerelease = versioning.IsPrerelease
        };

        var release = await context.Tools.GitHub.Client.Repository.Release.Create(repositoryInfo.Owner, repositoryInfo.RepositoryName, newRelease);
#if (hasArtifacts)
        await targetFiles
            .ForEachAsync(async file =>
            {
                await using var stream = file.GetStream();
                var asset = new ReleaseAssetUpload
                {
                    ContentType = "application/octet-stream",
                    FileName = file.Name,
                    RawData = stream
                };

                LogAssetUploading(context.Logger, asset.FileName);

                await context.Tools.GitHub.Client.Repository.Release.UploadAsset(release, asset, cancellationToken);
            }, cancellationToken)
            .ProcessInParallel();
#endif

        context.Summary.KeyValue("Deployment", "GitHub", release.HtmlUrl);
    }

    protected override async Task OnFailedAsync(IModuleContext context, Exception exception, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.Value;

        await context.Tools.Git.Commands.Remotes.PushAsync(new GitPushOptions
        {
            Delete = true,
            Arguments = ["origin", versioning.Version]
        }, cancellationToken: cancellationToken);
    }
#if (hasArtifacts)

    [LoggerMessage(LogLevel.Information, "Uploading the release asset {Asset}.")]
    private static partial void LogAssetUploading(ILogger logger, string asset);
#endif
}
