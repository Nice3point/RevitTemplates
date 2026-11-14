using ModularPipelines.Attributes;
using ModularPipelines.Conditions;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using Sourcy.DotNet;

namespace Build.Modules;

/// <summary>
///     Represents the pipeline step that tests the add-in for every Revit configuration of the solution.
/// </summary>
[SkipIf<IsCI>]
[DependsOn<ResolveConfigurationsModule>]
[DependsOn<CompileProjectModule>(Optional = true)]
public sealed class TestProjectModule : Module
{
    protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var configurationsResult = await context.GetModule<ResolveConfigurationsModule>();
        var configurations = configurationsResult.ValueOrDefault!;

        foreach (var configuration in configurations)
        {
            await context.SubModule(configuration, async () => await TestAsync(context, configuration, cancellationToken));
        }
    }

    /// <summary>
    ///     Runs the tests of the solution in the specified configuration.
    /// </summary>
    private static async Task<CommandResult> TestAsync(IModuleContext context, string configuration, CancellationToken cancellationToken)
    {
        return await context.DotNet().Test(new DotNetTestOptions
        {
            Solution = Solutions.Nice3point_Revit_AddIn__1.FullName,
            Configuration = configuration
        }, cancellationToken: cancellationToken);
    }
}
