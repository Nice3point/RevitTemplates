using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using Sourcy.DotNet;

namespace Build.Modules;

public sealed class CompileProjectsModule : Module<CommandResult>
{
    protected override async Task<CommandResult?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        return await context.DotNet().Build(new DotNetBuildOptions
        {
            ProjectSolution = Projects.Nice3point_Revit_Templates.FullName,
            Configuration = Configuration.Release,
            Verbosity = Verbosity.Minimal,
        }, cancellationToken);
    }
}