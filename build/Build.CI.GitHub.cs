using System.Text;
using Nuke.Common.Git;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.GitHub;
using Octokit;

sealed partial class Build
{
    Target PublishGitHub => _ => _
        .DependsOn(Pack)
        .Requires(() => GitHubToken)
        .Requires(() => GitRepository)
        .OnlyWhenStatic(() => IsServerBuild && GitRepository.IsOnMainOrMasterBranch())
        .Executes(async () =>
        {
            GitHubTasks.GitHubClient = new GitHubClient(new ProductHeaderValue(Solution.Name))
            {
                Credentials = new Credentials(GitHubToken)
            };

            var gitHubName = GitRepository.GetGitHubName();
            var gitHubOwner = GitRepository.GetGitHubOwner();

            ValidateRelease();

            var artifacts = Directory.GetFiles(ArtifactsDirectory, "*");
            var changelog = CreateGithubChangelog();
            Assert.NotEmpty(artifacts, "No artifacts were found to create the Release");

            var newRelease = new NewRelease(Version)
            {
                Name = Version,
                Body = changelog,
                TargetCommitish = GitRepository.Commit,
                Prerelease = Version.Contains("-beta") ||
                             Version.Contains("-dev") ||
                             Version.Contains("-preview")
            };

            var release = await GitHubTasks.GitHubClient.Repository.Release.Create(gitHubOwner, gitHubName, newRelease);
            await UploadArtifactsAsync(release, artifacts);
        });

    void ValidateRelease()
    {
        var tags = GitTasks.Git("describe --tags --abbrev=0 --always", logInvocation: false, logOutput: false);
        var latestTag = tags.First().Text;
        if (latestTag == GitRepository.Commit) return;

        Assert.False(latestTag == Version, $"A Release with the specified tag already exists in the repository: {Version}");
        Log.Information("Version: {Version}", Version);
    }

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

    string CreateGithubChangelog()
    {
        Assert.True(File.Exists(ChangeLogPath), $"Unable to locate the changelog file: {ChangeLogPath}");
        Log.Information("Changelog: {Path}", ChangeLogPath);

        var changelog = BuildChangelog();
        Assert.True(changelog.Length > 0, $"No version entry exists in the changelog: {Version}");

        WriteCompareUrl(changelog);
        return changelog.ToString();
    }

    void WriteCompareUrl(StringBuilder changelog)
    {
        var tags = GitTasks.Git("describe --tags --abbrev=0 --always", logInvocation: false, logOutput: false);
        var latestTag = tags.First().Text;
        if (latestTag == GitRepository.Commit) return;

        if (changelog[^1] != '\r' || changelog[^1] != '\n') changelog.AppendLine(Environment.NewLine);
        changelog.Append("Full changelog: ");
        changelog.Append(GitRepository.GetGitHubCompareTagsUrl(Version, latestTag));
    }
}