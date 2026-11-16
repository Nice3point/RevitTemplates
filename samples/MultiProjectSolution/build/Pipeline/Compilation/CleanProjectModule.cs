using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using Sourcy.DotNet;

namespace Build.Pipeline.Compilation;

/// <summary>
///     Represents the pipeline step that cleans the build output of the projects and the artifact directory.
/// </summary>
[SkipIf<OnCI>]
public sealed class CleanProjectModule(IOptions<BuildOptions> buildOptions) : Module
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var repository = await context.Tools.Git.Information.GetRequiredInfoAsync(cancellationToken);
        var rootDirectory = repository.Root;
        var outputDirectory = rootDirectory.GetFolder(buildOptions.Value.OutputDirectory);
        var buildOutputDirectories = rootDirectory
            .GetFolders(folder => folder.Name is "bin" or "obj")
            .Where(folder => folder.Parent != Projects.Build.Directory);

        foreach (var buildFolder in buildOutputDirectories)
        {
            buildFolder.Clean();
        }

        if (outputDirectory.Exists)
        {
            outputDirectory.Clean();
        }
    }
}
