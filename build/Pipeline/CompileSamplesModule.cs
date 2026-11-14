using ModularPipelines.Context;
using ModularPipelines.DotNet.Options;

namespace Build.Pipeline;

/// <summary>
///     Represents the pipeline step that compiles every sample solution for Revit 2027.
/// </summary>
public sealed class CompileSamplesModule : Module
{
    protected override async Task ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var repository = await context.Tools.Git.Information.GetRequiredInfoAsync(cancellationToken);
        var samplesFolder = repository.Root.GetFolder("samples");
        var sampleSolutions = samplesFolder.GetFiles(file => file.Extension == ".slnx");

        foreach (var sampleSolution in sampleSolutions)
        {
            await context.Tools.DotNet.BuildAsync(new DotNetBuildOptions
            {
                ProjectSolution = sampleSolution.Path,
                Configuration = "Release.R27"
            }, cancellationToken: cancellationToken);
        }
    }
}
