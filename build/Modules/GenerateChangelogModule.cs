using System.Text;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;
using Shouldly;
using File = ModularPipelines.FileSystem.File;

namespace Build.Modules;

/// <summary>
///     Generate the changelog for publishing the templates.
/// </summary>
[DependsOn<ResolveVersioningModule>]
public sealed class GenerateChangelogModule : Module<string>
{
    protected override async Task<string?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.Value!;
        
        var changelogFile = context.Git().RootDirectory.GetFile("Changelog.md");

        var changelog = await ParseChangelog(changelogFile, versioning.Version);
        changelog.Length.ShouldBePositive($"No version entry exists in the changelog: {versioning.Version}");

        return changelog.ToString();
    }

    /// <summary>
    ///     Parse the changelog file to extract the entries for a specific version.
    /// </summary>
    private static async Task<StringBuilder> ParseChangelog(File changelogFile, string version)
    {
        const string separator = "# ";

        var isChangelogEntryFound = false;
        var changelog = new StringBuilder();

        foreach (var line in await changelogFile.ReadLinesAsync())
        {
            if (isChangelogEntryFound)
            {
                if (line.StartsWith(separator)) break;

                changelog.AppendLine(line);
                continue;
            }

            if (line.StartsWith(separator) && line.Contains(version))
            {
                isChangelogEntryFound = true;
            }
        }

        TrimEmptyLines(changelog);
        return changelog;
    }

    /// <summary>
    ///     Remove empty lines from the beginning and end of the changelog builder.
    /// </summary>
    private static void TrimEmptyLines(StringBuilder changelog)
    {
        if (changelog.Length == 0) return;

        var start = 0;
        var end = changelog.Length - 1;

        while (start < changelog.Length && (changelog[start] == '\r' || changelog[start] == '\n')) start++;
        while (end >= start && (changelog[end] == '\r' || changelog[end] == '\n')) end--;

        if (end < changelog.Length - 1)
        {
            changelog.Remove(end + 1, changelog.Length - (end + 1));
        }

        if (start > 0)
        {
            changelog.Remove(0, start);
        }
    }
}