using System.Text.RegularExpressions;
using ModularPipelines.Context;

namespace Build.Pipeline.Publishing;

/// <summary>
///     Represents the pipeline step that formats the release notes for the NuGet package.
/// </summary>
/// <remarks>The step removes the Markdown syntax that NuGet does not render, and escapes the characters that the MSBuild property syntax reserves.</remarks>
[DependsOn<GenerateChangelogModule>]
public sealed partial class GenerateNugetChangelogModule : Module<string>
{
    /// <inheritdoc />
    protected override async Task<string> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var changelogResult = await context.GetModule<GenerateChangelogModule>();
        var changelog = changelogResult.Value;

        var formattedChangelog = changelog
            .Split(Environment.NewLine)
            .Where(line => !line.Contains("```"))
            .Where(line => !line.Contains("!["))
            .Select(line => HeadingRegex().Replace(line, string.Empty))
            .Select(line => ListItemRegex().Replace(line, "$1• "))
            .Select(line => line
                .Replace("**", string.Empty)
                .Replace("`", string.Empty)
                .Replace(";", "%3B")
                .Replace(",", "%2C"));

        return string.Join(Environment.NewLine, formattedChangelog);
    }

    /// <summary>
    ///     Gets the regular expression that matches the marker of a Markdown heading at the start of a line.
    /// </summary>
    [GeneratedRegex("^#{1,6} ")]
    private static partial Regex HeadingRegex();

    /// <summary>
    ///     Gets the regular expression that matches the marker of a Markdown list item at the start of a line, with its indentation.
    /// </summary>
    /// <remarks>A dash inside the text, as in <c>dotnet run -- pack</c>, is not a list marker.</remarks>
    [GeneratedRegex(@"^(\s*)[-*+] ")]
    private static partial Regex ListItemRegex();
}
