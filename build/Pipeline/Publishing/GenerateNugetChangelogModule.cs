using ModularPipelines.Context;

namespace Build.Pipeline.Publishing;

/// <summary>
///     Represents the pipeline step that formats the release notes for the NuGet package.
/// </summary>
/// <remarks>The step removes the Markdown syntax that NuGet does not render, and escapes the characters that the MSBuild property syntax reserves.</remarks>
[DependsOn<GenerateChangelogModule>]
public sealed class GenerateNugetChangelogModule : Module<string>
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
            .Select(line => line
                .Replace(";", "%3B")
                .Replace("- ", "• ")
                .Replace("**", string.Empty)
                .Replace("#### ", string.Empty)
                .Replace("### ", string.Empty)
                .Replace("## ", string.Empty)
                .Replace("# ", string.Empty)
                .Replace("* ", "• ")
                .Replace("+ ", "• ")
                .Replace("`", string.Empty)
                .Replace(",", "%2C"));

        return string.Join(Environment.NewLine, formattedChangelog);
    }
}
