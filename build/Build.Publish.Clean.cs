using Nuke.Common.Tools.Git;

sealed partial class Build
{
    Target CleanFailedRelease => _ => _
        .AssuredAfterFailure()
        .TriggeredBy(PublishGitHub)
        .Requires(() => ReleaseVersion)
        .OnlyWhenDynamic(() => FailedTargets.Contains(PublishGitHub) || FailedTargets.Contains(PublishNuget))
        .Executes(() =>
        {
            Log.Information("Cleaning failed GitHub release");
            GitTasks.Git($"push --delete origin {ReleaseVersion}", logInvocation: false);
        });
}