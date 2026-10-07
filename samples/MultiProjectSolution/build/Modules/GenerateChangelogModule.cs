using System.Text;
using System.Text.RegularExpressions;
using Build.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.GitHub.Extensions;
using ModularPipelines.Modules;
using Octokit;
using File = ModularPipelines.FileSystem.File;

namespace Build.Modules;

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
    protected override async Task<string?> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.ValueOrDefault!;

        if (string.IsNullOrEmpty(publishOptions.Value.ChangelogFile))
        {
            LogChangelogFileNotSpecified(context.Logger);
            return await GenerateReleaseNotesAsync(context, versioning);
        }

        var changelogFile = context.Git().RootDirectory.GetFile(publishOptions.Value.ChangelogFile);
        if (!changelogFile.Exists)
        {
            LogChangelogFileNotFound(context.Logger, changelogFile.Path);
            return await GenerateReleaseNotesAsync(context, versioning);
        }

        var changelog = await ParseChangelogAsync(changelogFile, versioning.Version);
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
    private static async Task<StringBuilder> ParseChangelogAsync(File changelogFile, string version)
    {
        const string separator = "# ";

        var versionPattern = $@"(?<![\w.-]){Regex.Escape(version)}(?![\w.-])";
        var isChangelogEntryFound = false;
        var changelog = new StringBuilder();

        await foreach (var line in changelogFile.ReadLinesAsync())
        {
            if (isChangelogEntryFound)
            {
                if (line.StartsWith(separator)) break;

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
        if (changelog.Length == 0) return;

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
            changelog.Remove(0, start);
        }
    }

    /// <summary>
    ///     Generates the release notes for the version by using the GitHub API.
    /// </summary>
    private static async Task<string?> GenerateReleaseNotesAsync(IModuleContext context, ResolveVersioningResult versioning)
    {
        var repositoryId = long.Parse(context.GitHub().EnvironmentVariables.RepositoryId!);

        var previousVersion = versioning.PreviousVersion;
        var isHashedVersion = previousVersion.Length >= 40 && previousVersion.All(character => char.IsDigit(character) || character is >= 'a' and <= 'f');

        var releaseNotes = await context.GitHub().Client.Repository.Release.GenerateReleaseNotes(repositoryId,
            new GenerateReleaseNotesRequest(versioning.Version)
            {
                PreviousTagName = isHashedVersion ? null : previousVersion
            });

        return releaseNotes.Body;
    }

    [LoggerMessage(LogLevel.Information, "The changelog file is not specified. GitHub generates the release notes.")]
    private static partial void LogChangelogFileNotSpecified(ILogger logger);

    [LoggerMessage(LogLevel.Warning, "The changelog file {ChangelogFile} was not found. GitHub generates the release notes.")]
    private static partial void LogChangelogFileNotFound(ILogger logger, string changelogFile);

    [LoggerMessage(LogLevel.Warning, "The changelog has no entry for version {Version}. GitHub generates the release notes.")]
    private static partial void LogChangelogEntryNotFound(ILogger logger, string version);
}
