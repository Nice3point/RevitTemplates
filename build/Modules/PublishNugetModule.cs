using Build.Options;
using EnumerableAsyncProcessor.Extensions;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using Shouldly;

namespace Build.Modules;

[DependsOn<PackProjectsModule>]
public sealed class PublishNugetModule(IOptions<PackOptions> packOptions, IOptions<NuGetOptions> nuGetOptions) : Module<CommandResult[]?>
{
    protected override async Task<CommandResult[]?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var outputFolder = context.Git().RootDirectory.GetFolder(packOptions.Value.OutputDirectory);
        var targetPackages = outputFolder.GetFiles(file => file.Extension == ".nupkg").ToArray();
        targetPackages.ShouldNotBeEmpty("No NuGet packages were found to publish");

        return await targetPackages
            .SelectAsync(async file => await context.DotNet().Nuget.Push(new DotNetNugetPushOptions
                {
                    Path = file,
                    ApiKey = nuGetOptions.Value.ApiKey,
                    Source = nuGetOptions.Value.Source
                }, cancellationToken),
                cancellationToken)
            .ProcessOneAtATime();
    }
}