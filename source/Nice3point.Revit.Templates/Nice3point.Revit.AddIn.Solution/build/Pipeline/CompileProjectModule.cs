using ModularPipelines.Context;
using ModularPipelines.DotNet.Options;
using Sourcy.DotNet;

namespace Build.Pipeline;

/// <summary>
///     Represents the pipeline step that compiles the add-in for every Revit configuration of the solution.
/// </summary>
[DependsOn<ResolveVersioningModule>]
[DependsOn<ResolveConfigurationsModule>]
#if (hasArtifacts)
[DependsOn<CleanProjectModule>(Optional = true)]
#endif
public sealed class CompileProjectModule : Module
{
    protected override async Task ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var configurationsResult = await context.GetModule<ResolveConfigurationsModule>();
        var versioning = versioningResult.Value;
        var configurations = configurationsResult.Value;

        foreach (var configuration in configurations)
        {
            await context.RunSubModuleAsync(configuration, token => CompileAsync(context, versioning, configuration, token), cancellationToken);
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
        await context.Tools.DotNet.BuildAsync(new DotNetBuildOptions
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
