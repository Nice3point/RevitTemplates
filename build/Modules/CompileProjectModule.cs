using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Modules;
using Sourcy.DotNet;

namespace Build.Modules;

/// <summary>
///     Represents the pipeline step that compiles the solution.
/// </summary>
[DependsOn<ResolveVersioningModule>]
public sealed class CompileProjectModule : Module
{
    protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.ValueOrDefault!;

        await context.DotNet().Build(new DotNetBuildOptions
        {
            ProjectSolution = Solutions.Nice3point_Revit_Templates.FullName,
            Configuration = "Release",
            Properties =
            [
                ("VersionPrefix", versioning.VersionPrefix),
                ("VersionSuffix", versioning.VersionSuffix ?? string.Empty)
            ]
        }, cancellationToken: cancellationToken);
    }
}
