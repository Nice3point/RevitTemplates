using Nuke.Common.Tools.Git;
using Nuke.Common.Utilities.Collections;

sealed partial class Build
{
    /// <summary>
    ///     Revert the repository to the previous state after failed release.
    /// </summary>
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