using Build.Pipeline.Compilation;
using Build.Pipeline.Versioning;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Options;
using Sourcy.DotNet;

namespace Build.Pipeline.Testing;

/// <summary>
///     Represents the pipeline step that tests the add-in for every Revit configuration of the solution.
/// </summary>
[SkipIf<OnCI>]
[DependsOn<ResolveConfigurationsModule>]
[DependsOn<CompileProjectModule>(Optional = true)]
public sealed class TestProjectModule : Module
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var configurationsResult = await context.GetModule<ResolveConfigurationsModule>();
        var configurations = configurationsResult.Value;

        foreach (var configuration in configurations)
        {
            await context.RunSubModuleAsync(configuration, token => TestAsync(context, configuration, token), cancellationToken);
        }
    }

    /// <summary>
    ///     Runs the tests of the solution in the specified configuration.
    /// </summary>
    private static async Task<CommandResult> TestAsync(IModuleContext context, string configuration, CancellationToken cancellationToken)
    {
        return await context.Tools.DotNet.TestAsync(new DotNetTestOptions
        {
            Solution = Solutions.Nice3point_Revit_AddIn__1.FullName,
            Configuration = configuration
        }, cancellationToken: cancellationToken);
    }
}
