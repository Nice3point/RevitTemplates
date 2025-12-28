using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;

namespace Build.Modules;

/// <summary>
///     Update the template Readme.md file for packaging.
/// </summary>
public sealed class UpdateTemplatesReadmeModule : Module<string>
{
    protected override async Task<string?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var readmePath = context.Git().RootDirectory.GetFile("Readme.md");
        var readme = await readmePath.ReadAsync(cancellationToken);

        const string startSymbol = "<p";
        const string endSymbol = "</p>\r\n\r\n";

        var logoStartIndex = readme.IndexOf(startSymbol, StringComparison.Ordinal);
        var logoEndIndex = readme.IndexOf(endSymbol, StringComparison.Ordinal);

        var nugetReadme = readme.Remove(logoStartIndex, logoEndIndex - logoStartIndex + endSymbol.Length);
        await readmePath.WriteAsync(nugetReadme, cancellationToken);

        return readme;
    }
}
