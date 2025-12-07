using ModularPipelines.Context;
using ModularPipelines.Modules;
using ModularPipelines.Attributes;

namespace Build.Modules;

[DependsOn<CreateChangelogModule>]
public sealed class CreatePackageChangelogModule : Module<string>
{
    protected override async Task<string?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var changelogModule = await GetModule<CreateChangelogModule>();

        var formattedChangelog = changelogModule.Value!.ToString()
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