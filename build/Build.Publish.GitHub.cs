using Nuke.Common.Tools.GitHub;
using Octokit;

sealed partial class Build
{
    Target PublishGitHub => _ => _
        .DependsOn(Pack)
        .Requires(() => ReleaseVersion)
        .OnlyWhenStatic(() => IsServerBuild)
        .Executes(async () =>
        {
            var gitHubName = GitRepository.GetGitHubName();
            var gitHubOwner = GitRepository.GetGitHubOwner();

            var changelog = CreateGithubChangelog();
            var artifacts = Directory.GetFiles(ArtifactsDirectory, "*");
            Assert.NotEmpty(artifacts, "No artifacts were found to create the Release");

            var newRelease = new NewRelease(ReleaseVersion)
            {
                Name = ReleaseVersion,
                Body = changelog,
                TargetCommitish = GitRepository.Commit,
                Prerelease = ReleaseVersion.Contains('-')
            };

            var release = await GitHubTasks.GitHubClient.Repository.Release.Create(gitHubOwner, gitHubName, newRelease);
            await UploadArtifactsAsync(release, artifacts);
        });

    static async Task UploadArtifactsAsync(Release createdRelease, IEnumerable<string> artifacts)
    {
        foreach (var file in artifacts)
        {
            var releaseAssetUpload = new ReleaseAssetUpload
            {
                ContentType = "application/x-binary",
                FileName = Path.GetFileName(file),
                RawData = File.OpenRead(file)
            };

            await GitHubTasks.GitHubClient.Repository.Release.UploadAsset(createdRelease, releaseAssetUpload);
            Log.Information("Artifact: {Path}", file);
        }
    }
}