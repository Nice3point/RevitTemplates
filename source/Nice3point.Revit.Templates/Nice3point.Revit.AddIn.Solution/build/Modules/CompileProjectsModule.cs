using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using Sourcy.DotNet;

namespace Build.Modules;

/// <summary>
///     Compile the add-in for each supported Revit configuration.
/// </summary>
[DependsOn<ResolveVersioningModule>]
[DependsOn<ResolveConfigurationsModule>]
public sealed class CompileProjectsModule : Module
{
    protected override async Task<IDictionary<string, object>?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await GetModule<ResolveVersioningModule>();
        var configurationsResult = await GetModule<ResolveConfigurationsModule>();
        var versioning = versioningResult.Value!;
        var configurations = configurationsResult.Value!;

        foreach (var configuration in configurations)
        {
            await SubModule(configuration, async () => await CompileAsync(context, versioning, configuration, cancellationToken));
        }

        return await NothingAsync();
    }

    /// <summary>
    ///     Compile the add-in project for the specified configuration.
    /// </summary>
    private static async Task<CommandResult> CompileAsync(
        IPipelineContext context,
        ResolveVersioningResult versioning,
        string configuration,
        CancellationToken cancellationToken)
    {
        return await context.DotNet().Build(new DotNetBuildOptions
        {
            ProjectSolution = Solutions.____Nice3point.Revit.AddIn_2.FullName,
            Configuration = configuration,
            Properties =
            [
                ("VersionPrefix", versioning.VersionPrefix),
                ("VersionSuffix", versioning.VersionSuffix!)
            ]
        }, cancellationToken);
    }
}