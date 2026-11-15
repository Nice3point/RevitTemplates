using Build.Pipeline.Packaging;
using Build.Pipeline.Testing;
using Build.Pipeline.Versioning;
using EnumerableAsyncProcessor.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.Git.Options;
using ModularPipelines.GitHub;
using Octokit;
using Shouldly;

namespace Build.Pipeline.Publishing;

/// <summary>
///     Represents the pipeline step that publishes the GitHub release of the packages.
/// </summary>
/// <remarks>If the publication fails, the step deletes the version tag from the remote repository.</remarks>
[SkipIfNoGitHubToken]
[DependsOn<ResolveVersioningModule>]
[DependsOn<GenerateGitHubChangelogModule>]
[DependsOn<PackTemplatesModule>(Optional = true)]
[DependsOn<PackSdkModule>(Optional = true)]
[DependsOn<TestTemplatesModule>(Optional = true)]
[DependsOn<PublishNugetModule>(Optional = true)]
public sealed partial class PublishGithubModule(IOptions<BuildOptions> buildOptions) : Module
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var changelogResult = await context.GetModule<GenerateGitHubChangelogModule>();

        var versioning = versioningResult.Value;
        var changelog = changelogResult.Value;

        var repository = await context.Tools.Git.Information.GetRequiredInfoAsync(cancellationToken);
        var outputFolder = repository.Root.GetFolder(buildOptions.Value.OutputDirectory);
        var targetFiles = outputFolder.ListFiles().ToArray();
        targetFiles.ShouldNotBeEmpty($"Cannot publish the release. No packages were found in '{outputFolder.Path}'. Run the build with the 'pack' argument before publishing.");

        var repositoryInfo = context.Tools.GitHub.RepositoryInfo;
        var newRelease = new NewRelease(versioning.Version)
        {
            Name = versioning.Version,
            Body = changelog,
            TargetCommitish = repository.LastCommitSha,
            Prerelease = versioning.IsPrerelease
        };

        var release = await context.Tools.GitHub.Client.Repository.Release.Create(repositoryInfo.Owner, repositoryInfo.RepositoryName, newRelease);
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
            .ProcessOneAtATime();

        context.Summary.KeyValue("Deployment", "GitHub", release.HtmlUrl);
    }

    /// <inheritdoc />
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

    [LoggerMessage(LogLevel.Information, "Uploading the release asset {Asset}")]
    private static partial void LogAssetUploading(ILogger logger, string asset);
}
