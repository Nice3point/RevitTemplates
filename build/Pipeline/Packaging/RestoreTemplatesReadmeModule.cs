using ModularPipelines.Context;

namespace Build.Pipeline.Packaging;

/// <summary>
///     Represents the pipeline step that restores the readme after the packages are packed.
/// </summary>
/// <remarks>The step runs after a failed pack as well, and the repository keeps the original readme.</remarks>
[DependsOn<UpdateTemplatesReadmeModule>]
[DependsOn<PackSdkModule>(Optional = true)]
[DependsOn<PackTemplatesModule>(Optional = true)]
public sealed class RestoreTemplatesReadmeModule : Module
{
    /// <inheritdoc />
    protected override void Configure(ModuleConfigurationBuilder module)
    {
        module
            .WithAlwaysRun()
            .WithSkipWhen(async (context, _) =>
            {
                var nugetReadmeResult = await context.GetModule<UpdateTemplatesReadmeModule>();
                return nugetReadmeResult is not ModuleResult<string>.Success;
            });
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var nugetReadmeResult = await context.GetModule<UpdateTemplatesReadmeModule>();
        if (!nugetReadmeResult.TryGetValue(out var nugetReadme))
        {
            return;
        }

        var repository = await context.Tools.Git.Information.GetRequiredInfoAsync(cancellationToken);
        var readmePath = repository.Root.GetFile("README.md");
        await readmePath.WriteAsync(nugetReadme, cancellationToken);
    }
}
