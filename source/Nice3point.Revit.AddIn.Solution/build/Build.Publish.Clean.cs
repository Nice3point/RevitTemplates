using Nuke.Common.Tools.Git;

sealed partial class Build
{
    Target CleanGitHubPublication => _ => _
        .Unlisted()
        .AssuredAfterFailure()
        .Requires(() => ReleaseVersion)
        .TriggeredBy(PublishGitHub)
        .OnlyWhenDynamic(() => ScheduledTargets.Contains(PublishGitHub) && FailedTargets.Contains(PublishGitHub))
        .Executes(() =>
        {
            Log.Information("Cleaning failed GitHub release");
            GitTasks.Git($"push --delete origin {ReleaseVersion}", logInvocation: false);
        });
}