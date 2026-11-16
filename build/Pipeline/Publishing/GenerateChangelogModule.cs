using System.Text;
using System.Text.RegularExpressions;
using Build.Pipeline.Versioning;
using ModularPipelines.Context;
using ModularPipelines.FileSystem;
using Shouldly;

namespace Build.Pipeline.Publishing;

/// <summary>
///     Represents the pipeline step that reads the release notes of the version from the changelog.
/// </summary>
[DependsOn<ResolveVersioningModule>]
public sealed class GenerateChangelogModule : Module<string>
{
    /// <inheritdoc />
    protected override async Task<string> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.Value;

        var repository = await context.Tools.Git.Information.GetRequiredInfoAsync(cancellationToken);
        var changelogFile = repository.Root.GetFile("CHANGELOG.md");

        var changelog = await ParseChangelogAsync(changelogFile, versioning.Version, cancellationToken);
        changelog.Length.ShouldBePositive($"Cannot publish the release. CHANGELOG.md has no entry for version '{versioning.Version}'. Add a '# {versioning.Version}' heading with the release notes.");

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
}
