using System.Text;
using System.Text.RegularExpressions;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;
using Shouldly;
using File = ModularPipelines.FileSystem.File;

namespace Build.Modules;

/// <summary>
///     Represents the pipeline step that reads the release notes of the version from the changelog.
/// </summary>
[DependsOn<ResolveVersioningModule>]
public sealed class GenerateChangelogModule : Module<string>
{
    protected override async Task<string?> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.ValueOrDefault!;

        var changelogFile = context.Git().RootDirectory.GetFile("CHANGELOG.md");

        var changelog = await ParseChangelogAsync(changelogFile, versioning.Version);
        changelog.Length.ShouldBePositive($"Cannot publish the release. CHANGELOG.md has no entry for version '{versioning.Version}'. Add a '# {versioning.Version}' heading with the release notes.");

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
            changelog.Remove(0, start);
        }
    }
}
