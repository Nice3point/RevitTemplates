using Build.Attributes;
using Build.Options;
using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;

namespace Build.Modules;

[SkipIfContinuousIntegrationBuild]
public sealed class CleanProjectsModule(IOptions<PackOptions> packOptions) : Module
{
    protected override async Task<IDictionary<string, object>?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var outputDirectory = context.Git().RootDirectory.GetFolder(packOptions.Value.OutputDirectory);
        if (outputDirectory.Exists)
        {
            outputDirectory.Delete();
        }

        return await NothingAsync();
    }
}