using Nuke.Common.Tools.Git;
using Nuke.Common.Utilities.Collections;

sealed partial class Build
{
    Target CleanFailedRelease => _ => _
        .Unlisted()
        .AssuredAfterFailure()
        .TriggeredBy(PublishGitHub)
        .OnlyWhenStatic(() => IsServerBuild)
        .OnlyWhenDynamic(() => !FailedTargets.IsEmpty())
        .Executes(() =>
        {
            Log.Information("Cleaning failed GitHub release");
            GitTasks.Git($"push --delete origin {ReleaseVersion}", logInvocation: false);
        });
}