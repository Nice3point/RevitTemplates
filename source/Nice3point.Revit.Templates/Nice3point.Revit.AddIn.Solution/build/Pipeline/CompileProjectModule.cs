using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Modules;
using Sourcy.DotNet;

namespace Build.Modules;

/// <summary>
///     Represents the pipeline step that compiles the add-in for every Revit configuration of the solution.
/// </summary>
[DependsOn<ResolveVersioningModule>]
[DependsOn<ResolveConfigurationsModule>]
[DependsOn<CleanProjectModule>(Optional = true)]
public sealed class CompileProjectModule : Module
{
    protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var configurationsResult = await context.GetModule<ResolveConfigurationsModule>();
        var versioning = versioningResult.ValueOrDefault!;
        var configurations = configurationsResult.ValueOrDefault!;

        foreach (var configuration in configurations)
        {
            await context.SubModule(configuration, async () => await CompileAsync(context, versioning, configuration, cancellationToken));
        }
    }

    /// <summary>
    ///     Compiles the solution in the specified configuration.
    /// </summary>
    private static async Task CompileAsync(
        IModuleContext context,
        ResolveVersioningResult versioning,
        string configuration,
        CancellationToken cancellationToken)
    {
        await context.DotNet().Build(new DotNetBuildOptions
        {
            ProjectSolution = Solutions.Nice3point_Revit_AddIn__1.FullName,
            Configuration = configuration,
            Properties =
            [
                ("VersionPrefix", versioning.VersionPrefix),
                ("VersionSuffix", versioning.VersionSuffix ?? string.Empty)
            ]
        }, cancellationToken: cancellationToken);
    }
}
