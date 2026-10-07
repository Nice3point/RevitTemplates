using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;

namespace Build.Modules;

/// <summary>
///     Represents the pipeline step that removes the logo block from the readme of the NuGet packages.
/// </summary>
/// <remarks>The step returns the original readme, and <see cref="RestoreTemplatesReadmeModule" /> writes it back after the packages are packed.</remarks>
public sealed class UpdateTemplatesReadmeModule : Module<string>
{
    protected override async Task<string?> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var readmePath = context.Git().RootDirectory.GetFile("README.md");
        var readme = await readmePath.ReadAsync(cancellationToken);

        const string startSymbol = "<p";
        const string endSymbol = "</p>";

        var logoStartIndex = readme.IndexOf(startSymbol, StringComparison.Ordinal);
        if (logoStartIndex < 0)
        {
            throw new InvalidOperationException("Cannot prepare the NuGet readme. README.md has no logo block that starts with '<p'.");
        }

        var logoEndIndex = readme.IndexOf(endSymbol, logoStartIndex, StringComparison.Ordinal);
        if (logoEndIndex < 0)
        {
            throw new InvalidOperationException("Cannot prepare the NuGet readme. The logo block of README.md has no closing '</p>' tag.");
        }

        logoEndIndex += endSymbol.Length;
        while (logoEndIndex < readme.Length && readme[logoEndIndex] is '\r' or '\n')
        {
            logoEndIndex++;
        }

        var nugetReadme = readme.Remove(logoStartIndex, logoEndIndex - logoStartIndex);
        await readmePath.WriteAsync(nugetReadme, cancellationToken);

        return readme;
    }
}
