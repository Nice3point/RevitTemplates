#if (hasArtifacts)
using Build.Options;
using EnumerableAsyncProcessor.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
#endif
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Git.Options;
using ModularPipelines.GitHub.Attributes;
using ModularPipelines.GitHub.Extensions;
using ModularPipelines.Modules;
using Octokit;
#if (hasArtifacts)
using Shouldly;
#endif

namespace Build.Modules;

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
    protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var changelogResult = await context.GetModule<GenerateGitHubChangelogModule>();
        var versioning = versioningResult.ValueOrDefault!;
        var changelog = changelogResult.ValueOrDefault!;
#if (hasArtifacts)

        var outputFolder = context.Git().RootDirectory.GetFolder(buildOptions.Value.OutputDirectory);
        var targetFiles = outputFolder.ListFiles().ToArray();
        targetFiles.ShouldNotBeEmpty($"Cannot publish the release. No artifacts were found in '{outputFolder.Path}'. Run the build with the 'pack' argument before publishing.");
#endif

        var repositoryInfo = context.GitHub().RepositoryInfo;
        var newRelease = new NewRelease(versioning.Version)
        {
            Name = versioning.Version,
            Body = changelog,
            TargetCommitish = context.Git().Information.LastCommitSha,
            Prerelease = versioning.IsPrerelease
        };

        var release = await context.GitHub().Client.Repository.Release.Create(repositoryInfo.Owner, repositoryInfo.RepositoryName, newRelease);
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

                await context.GitHub().Client.Repository.Release.UploadAsset(release, asset, cancellationToken);
            }, cancellationToken)
            .ProcessInParallel();
#endif

        context.Summary.KeyValue("Deployment", "GitHub", release.HtmlUrl);
    }

    protected override async Task OnFailedAsync(IModuleContext context, Exception exception, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.ValueOrDefault!;

        await context.Git().Commands.Push(new GitPushOptions
        {
            Delete = true,
            Arguments = ["origin", versioning.Version]
        }, token: cancellationToken);
    }
#if (hasArtifacts)

    [LoggerMessage(LogLevel.Information, "Uploading the release asset {Asset}.")]
    private static partial void LogAssetUploading(ILogger logger, string asset);
#endif
}
