using Nuke.Common.Tools.Git;

sealed partial class Build
{
    Target CleanFailedRelease => _ => _
        .Unlisted()
        .AssuredAfterFailure()
        .Requires(() => ReleaseVersion)
        .TriggeredBy(PublishGitHub, PublishNuget)
        .OnlyWhenDynamic(() => (ScheduledTargets.Contains(PublishGitHub) ||
                                ScheduledTargets.Contains(PublishNuget)) &&
                               (FailedTargets.Contains(PublishGitHub) ||
                                FailedTargets.Contains(PublishNuget)))
        .Executes(() =>
        {
            Log.Information("Cleaning failed GitHub release");
            GitTasks.Git($"push --delete origin {ReleaseVersion}", logInvocation: false);
        });
}