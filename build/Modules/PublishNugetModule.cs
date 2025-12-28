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

[DependsOn<PackTemplatesModule>]
public sealed class PublishNugetModule(IOptions<BuildOptions> buildOptions, IOptions<NuGetOptions> nuGetOptions) : Module<CommandResult[]?>
{
    protected override async Task<CommandResult[]?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var outputFolder = context.Git().RootDirectory.GetFolder(buildOptions.Value.OutputDirectory);
        var targetPackages = outputFolder.GetFiles(file => file.Extension == ".nupkg" &&
                                                           !file.Name.Contains("sdk", StringComparison.OrdinalIgnoreCase)).ToArray();
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