using System.Text;
using Build.Options;
using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;
using Shouldly;
using File = ModularPipelines.FileSystem.File;

namespace Build.Modules;

public sealed class GenerateChangelogModule(IOptions<PackOptions> packOptions) : Module<string>
{
    protected override async Task<string?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var changelogFile = context.Git().RootDirectory.GetFile("Changelog.md");
        var version = packOptions.Value.Version;

        var changelog = await BuildChangelog(changelogFile, version);
        changelog.Length.ShouldBePositive($"No version entry exists in the changelog: {version}");

        return changelog.ToString();
    }

    private static async Task<StringBuilder> BuildChangelog(File changelogFile, string version)
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