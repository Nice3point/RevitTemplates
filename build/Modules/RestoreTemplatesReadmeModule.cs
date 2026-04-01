using ModularPipelines.Attributes;
using ModularPipelines.Configuration;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;

namespace Build.Modules;

/// <summary>
///     Restore the readme.
/// </summary>
[DependsOn<UpdateTemplatesReadmeModule>]
[DependsOn<PackSdkModule>(Optional = true)]
[DependsOn<PackTemplatesModule>(Optional = true)]
public sealed class RestoreReadmeModule : Module
{
    protected override ModuleConfiguration Configure() => ModuleConfiguration.Create()
        .WithAlwaysRun()
        .WithSkipWhen(async context =>
        {
            var nugetReadmeModule = await context.GetModule<UpdateTemplatesReadmeModule>();
            return !nugetReadmeModule.IsSuccess;
        })
        .Build();

    protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var nugetReadmeResult = await context.GetModule<UpdateTemplatesReadmeModule>();
        if (!nugetReadmeResult.IsSuccess)
        {
            return;
        }

        var nugetReadme = nugetReadmeResult.ValueOrDefault!;
        var readmePath = context.Git().RootDirectory.GetFile("Readme.md");
        await readmePath.WriteAsync(nugetReadme, cancellationToken);
    }
}