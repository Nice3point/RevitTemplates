using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;

namespace Build.Modules;

/// <summary>
///     Compile every sample solution.
/// </summary>
public sealed class CompileSamplesModule : Module
{
    protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var samplesFolder = context.Git().RootDirectory.GetFolder("samples");
        var sampleSolutions = samplesFolder.GetFiles(file => file.Extension == ".slnx");

        foreach (var sampleSolution in sampleSolutions)
        {
            await context.DotNet().Build(new DotNetBuildOptions
            {
                ProjectSolution = sampleSolution.Path,
                Configuration = "Release.R27"
            }, cancellationToken: cancellationToken);
        }
    }
}