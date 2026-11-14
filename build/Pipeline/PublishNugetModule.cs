using Build.Options;
using EnumerableAsyncProcessor.Extensions;
using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Options;
using Shouldly;

namespace Build.Pipeline;

/// <summary>
///     Represents the pipeline step that pushes the packages to the NuGet package source.
/// </summary>
[DependsOn<PackTemplatesModule>(Optional = true)]
[DependsOn<PackSdkModule>(Optional = true)]
[DependsOn<TestTemplatesModule>(Optional = true)]
public sealed class PublishNugetModule(IOptions<BuildOptions> buildOptions, IOptions<NuGetOptions> nuGetOptions) : Module
{
    protected override async Task ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var repository = await context.Tools.Git.Information.GetRequiredInfoAsync(cancellationToken);
        var outputFolder = repository.Root.GetFolder(buildOptions.Value.OutputDirectory);
        var targetPackages = outputFolder.GetFiles(file => file.Extension == ".nupkg").ToArray();
        targetPackages.ShouldNotBeEmpty($"Cannot push the packages. No NuGet packages were found in '{outputFolder.Path}'. Run the build with the 'pack' argument before publishing.");

        await targetPackages
            .ForEachAsync(async file => await context.Tools.DotNet.NuGet.PushAsync(new DotNetNuGetPushOptions([file.Path])
                {
                    ApiKey = nuGetOptions.Value.ApiKey,
                    Source = nuGetOptions.Value.Source
                }, cancellationToken: cancellationToken),
                cancellationToken)
            .ProcessOneAtATime();
    }
}
