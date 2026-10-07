using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.Modules;

namespace Build.Modules;

/// <summary>
///     Represents the pipeline step that formats the release notes for the NuGet package.
/// </summary>
/// <remarks>The step removes the Markdown syntax that NuGet does not render, and escapes the characters that the MSBuild property syntax reserves.</remarks>
[DependsOn<GenerateChangelogModule>]
public sealed class GenerateNugetChangelogModule : Module<string>
{
    protected override async Task<string?> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var changelogResult = await context.GetModule<GenerateChangelogModule>();
        var changelog = changelogResult.ValueOrDefault!;

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
