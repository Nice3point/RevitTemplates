using System.Text;
using System.Text.RegularExpressions;
using Build.Pipeline.Versioning;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.FileSystem;
using Octokit;

namespace Build.Pipeline.Publishing;

/// <summary>
///     Represents the pipeline step that resolves the release notes of the version.
/// </summary>
/// <remarks>
///     The step reads the release notes from the version entry in <see cref="PublishOptions.ChangelogFile" />.
///     If the file or the entry doesn't exist, GitHub generates the release notes.
/// </remarks>
[DependsOn<ResolveVersioningModule>]
public sealed partial class GenerateChangelogModule(IOptions<PublishOptions> publishOptions) : Module<string>
{
    /// <inheritdoc />
    protected override async Task<string> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.Value;

        if (string.IsNullOrEmpty(publishOptions.Value.ChangelogFile))
        {
            LogChangelogFileNotSpecified(context.Logger);
            return await GenerateReleaseNotesAsync(context, versioning);
        }

        var repository = await context.Tools.Git.Information.GetRequiredInfoAsync(cancellationToken);
        var changelogFile = repository.Root.GetFile(publishOptions.Value.ChangelogFile);
        if (!changelogFile.Exists)
        {
            LogChangelogFileNotFound(context.Logger, changelogFile.Path);
            return await GenerateReleaseNotesAsync(context, versioning);
        }

        var changelog = await ParseChangelogAsync(changelogFile, versioning.Version, cancellationToken);
        if (changelog.Length == 0)
        {
            LogChangelogEntryNotFound(context.Logger, versioning.Version);
            return await GenerateReleaseNotesAsync(context, versioning);
        }

        return changelog.ToString();
    }

    /// <summary>
    ///     Reads the entry of the specified version from the changelog file.
    /// </summary>
    /// <remarks>The entry starts at the heading that contains the version and ends before the next heading.</remarks>
    private static async Task<StringBuilder> ParseChangelogAsync(FilePath changelogFile, string version, CancellationToken cancellationToken)
    {
        const string separator = "# ";

        var versionPattern = $@"(?<![\w.-]){Regex.Escape(version)}(?![\w.-])";
        var isChangelogEntryFound = false;
        var changelog = new StringBuilder();

        await foreach (var line in changelogFile.ReadLinesAsync(cancellationToken))
        {
            if (isChangelogEntryFound)
            {
                if (line.StartsWith(separator))
                {
                    break;
                }

                changelog.AppendLine(line);
                continue;
            }

            if (line.StartsWith(separator) && Regex.IsMatch(line, versionPattern))
            {
                isChangelogEntryFound = true;
            }
        }

        TrimEmptyLines(changelog);
        return changelog;
    }

    /// <summary>
    ///     Removes the empty lines from the start and the end of the changelog.
    /// </summary>
    private static void TrimEmptyLines(StringBuilder changelog)
    {
        if (changelog.Length == 0)
        {
            return;
        }

        var start = 0;
        var end = changelog.Length - 1;

        while (start < changelog.Length && changelog[start] is '\r' or '\n')
        {
            start++;
        }

        while (end >= start && changelog[end] is '\r' or '\n')
        {
            end--;
        }

        if (end < changelog.Length - 1)
        {
            changelog.Remove(end + 1, changelog.Length - (end + 1));
        }

        if (start > 0)
        {
            changelog.Remove(startIndex: 0, start);
        }
    }

    /// <summary>
    ///     Generates the release notes for the version by using the GitHub API.
    /// </summary>
    private static async Task<string> GenerateReleaseNotesAsync(IModuleContext context, ResolveVersioningResult versioning)
    {
        var repositoryId = long.Parse(context.Tools.GitHub.EnvironmentVariables.RepositoryId!);
        var previousTagName = versioning.PreviousRelease switch
        {
            ReleaseTag tag => tag.Name,
            RootCommit => null
        };

        var releaseNotes = await context.Tools.GitHub.Client.Repository.Release.GenerateReleaseNotes(repositoryId,
            new GenerateReleaseNotesRequest(versioning.Version)
            {
                PreviousTagName = previousTagName
            });

        return releaseNotes.Body;
    }

    [LoggerMessage(LogLevel.Information, "The changelog file is not specified; GitHub generates the release notes")]
    private static partial void LogChangelogFileNotSpecified(ILogger logger);

    [LoggerMessage(LogLevel.Warning, "Changelog file {ChangelogFile} was not found; GitHub generates the release notes")]
    private static partial void LogChangelogFileNotFound(ILogger logger, string changelogFile);

    [LoggerMessage(LogLevel.Warning, "Changelog entry of version {Version} was not found; GitHub generates the release notes")]
    private static partial void LogChangelogEntryNotFound(ILogger logger, string version);
}
