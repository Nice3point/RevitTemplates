using Microsoft.VisualStudio.SolutionPersistence.Model;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;
using Shouldly;

namespace Build.Modules;

/// <summary>
///     Resolve solution configurations required to compile the add-in for all supported Revit versions.
/// </summary>
public sealed class ResolveConfigurationsModule : Module<string[]>
{
    protected override async Task<string[]?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var solutionModel = await LoadSolutionModelAsync(context, cancellationToken);
        var configurations = solutionModel.BuildTypes
            .Where(configuration => configuration.Contains("Release.R", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        configurations.ShouldNotBeEmpty("No solution configurations have been found");

        return configurations;
    }

    private static async Task<SolutionModel> LoadSolutionModelAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var solution = context.Git().RootDirectory.FindFile(file => file.Extension == ".slnx");
        if (solution is not null)
        {
            return await SolutionSerializers.SlnXml.OpenAsync(solution.GetStream(), cancellationToken);
        }

        solution = context.Git().RootDirectory.FindFile(file => file.Extension == ".sln");
        solution.ShouldNotBeNull("Solution file not found.");

        return await SolutionSerializers.SlnFileV12.OpenAsync(solution.GetStream(), cancellationToken);
    }
}