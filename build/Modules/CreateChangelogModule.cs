using System.Text;
using Build.Options;
using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;
using Shouldly;
using File = ModularPipelines.FileSystem.File;

namespace Build.Modules;

public sealed class CreateChangelogModule(IOptions<PackOptions> packOptions) : Module<StringBuilder>
{
    protected override async Task<StringBuilder?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var changelogFile = context.Git().RootDirectory.GetFile("Changelog.md");
        var version = packOptions.Value.Version;

        var changelog = await BuildChangelog(changelogFile, version);
        changelog.Length.ShouldBePositive($"No version entry exists in the changelog: {version}");

        return changelog;
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

        while (changelog[^1] == '\r' || changelog[^1] == '\n')
        {
            changelog.Remove(changelog.Length - 1, 1);
        }

        while (changelog[0] == '\r' || changelog[0] == '\n')
        {
            changelog.Remove(0, 1);
        }
    }
}