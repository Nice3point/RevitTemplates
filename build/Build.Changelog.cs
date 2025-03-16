using System.Text;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Utilities;

sealed partial class Build
{
    StringBuilder CreateChangelogBuilder()
    {
        Assert.True(File.Exists(ChangelogPath), $"Unable to locate the changelog file: {ChangelogPath}");
        Log.Information("Changelog: {Path}", ChangelogPath);

        var changelog = BuildChangelog();
        Assert.True(changelog.Length > 0, $"No version entry exists in the changelog: {ReleaseVersion}");
        return changelog;
    }

    string CreateNugetChangelog()
    {
        var builder = CreateChangelogBuilder();

        return builder.ToString()
            .Split(Environment.NewLine)
            .Where(line => !line.Contains("```"))
            .Where(line => !line.Contains("!["))
            .Select(line => line.Replace(";", "%3B")
                .Replace("- ", "• ")
                .Replace("**", string.Empty)
                .Replace("#### ", string.Empty)
                .Replace("### ", string.Empty)
                .Replace("## ", string.Empty)
                .Replace("# ", string.Empty)
                .Replace("* ", "• ")
                .Replace("+ ", "• ")
                .Replace("`", string.Empty)
                .Replace(",", "%2C"))
            .JoinNewLine();
    }

    string CreateGithubChangelog()
    {
        var builder = CreateChangelogBuilder();

        WriteGitHubCompareUrl(builder);
        return builder.ToString();
    }

    void WriteGitHubCompareUrl(StringBuilder changelog)
    {
        var tags = GitTasks
            .Git("tag --list", logInvocation: false, logOutput: false)
            .ToArray();

        if (tags.Length < 2) return;

        if (changelog[^1] != '\r' || changelog[^1] != '\n') changelog.AppendLine(Environment.NewLine);
        changelog.Append("Full changelog: ");
        changelog.Append(GitRepository.GetGitHubCompareTagsUrl(tags[^1].Text, tags[^2].Text));
    }

    StringBuilder BuildChangelog()
    {
        const string separator = "# ";

        var hasEntry = false;
        var changelog = new StringBuilder();
        foreach (var line in File.ReadLines(ChangelogPath))
        {
            if (hasEntry)
            {
                if (line.StartsWith(separator)) break;

                changelog.AppendLine(line);
                continue;
            }

            if (line.StartsWith(separator) && line.Contains(ReleaseVersion))
            {
                hasEntry = true;
            }
        }

        TrimEmptyLines(changelog);
        return changelog;
    }

    static void TrimEmptyLines(StringBuilder builder)
    {
        if (builder.Length == 0) return;

        while (builder[^1] == '\r' || builder[^1] == '\n')
        {
            builder.Remove(builder.Length - 1, 1);
        }

        while (builder[0] == '\r' || builder[0] == '\n')
        {
            builder.Remove(0, 1);
        }
    }
}