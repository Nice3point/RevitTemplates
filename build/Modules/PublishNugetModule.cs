using Build.Options;
using EnumerableAsyncProcessor.Extensions;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;
using Shouldly;

namespace Build.Modules;

/// <summary>
///     Represents the pipeline step that pushes the packages to the NuGet package source.
/// </summary>
[DependsOn<PackTemplatesModule>(Optional = true)]
[DependsOn<PackSdkModule>(Optional = true)]
[DependsOn<TestTemplatesModule>(Optional = true)]
public sealed class PublishNugetModule(IOptions<BuildOptions> buildOptions, IOptions<NuGetOptions> nuGetOptions) : Module
{
    protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var outputFolder = context.Git().RootDirectory.GetFolder(buildOptions.Value.OutputDirectory);
        var targetPackages = outputFolder.GetFiles(file => file.Extension == ".nupkg").ToArray();
        targetPackages.ShouldNotBeEmpty($"Cannot push the packages. No NuGet packages were found in '{outputFolder.Path}'. Run the build with the 'pack' argument before publishing.");

        await targetPackages
            .ForEachAsync(async file => await context.DotNet().Nuget.Push(new DotNetNugetPushOptions
                {
                    Path = file,
                    ApiKey = nuGetOptions.Value.ApiKey,
                    Source = nuGetOptions.Value.Source
                }, cancellationToken: cancellationToken),
                cancellationToken)
            .ProcessOneAtATime();
    }
}
