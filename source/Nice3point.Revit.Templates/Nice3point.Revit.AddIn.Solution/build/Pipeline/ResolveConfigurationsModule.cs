using Microsoft.VisualStudio.SolutionPersistence.Model;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;
using Shouldly;

namespace Build.Modules;

/// <summary>
///     Represents the pipeline step that resolves the solution configurations for the supported Revit versions.
/// </summary>
/// <remarks>The step selects the configurations whose names contain <c>Release.R</c>, for example, <c>Release.R26</c>.</remarks>
public sealed class ResolveConfigurationsModule : Module<string[]>
{
    protected override async Task<string[]?> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var solutionModel = await LoadSolutionModelAsync(context, cancellationToken);
        var configurations = solutionModel.BuildTypes
            .Where(configuration => configuration.Contains("Release.R", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        configurations.ShouldNotBeEmpty("Cannot resolve the Revit configurations. The solution has no configuration targeting a Revit version. Add a configuration such as 'Release.R26' to the solution.");

        return configurations;
    }

    /// <summary>
    ///     Loads the solution from the repository. If the repository contains both formats, the <c>.slnx</c> file takes precedence.
    /// </summary>
    private static async Task<SolutionModel> LoadSolutionModelAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var solution = context.Git().RootDirectory.FindFile(file => file.Extension == ".slnx");
        if (solution is not null)
        {
            await using var slnxStream = solution.GetStream();
            return await SolutionSerializers.SlnXml.OpenAsync(slnxStream, cancellationToken);
        }

        solution = context.Git().RootDirectory.FindFile(file => file.Extension == ".sln");
        solution.ShouldNotBeNull("Cannot resolve the Revit configurations. No .slnx or .sln file was found in the repository.");

        await using var slnStream = solution.GetStream();
        return await SolutionSerializers.SlnFileV12.OpenAsync(slnStream, cancellationToken);
    }
}
