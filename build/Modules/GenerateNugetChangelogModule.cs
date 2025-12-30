using ModularPipelines.Context;
using ModularPipelines.Modules;
using ModularPipelines.Attributes;

namespace Build.Modules;

/// <summary>
///     Generate and format the changelog for publishing on the NuGet.
/// </summary>
[DependsOn<GenerateChangelogModule>]
public sealed class GenerateNugetChangelogModule : Module<string>
{
    protected override async Task<string?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var changelogResult = await GetModule<GenerateChangelogModule>();
        var changelog = changelogResult.Value!;

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