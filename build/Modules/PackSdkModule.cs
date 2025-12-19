using Build.Options;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using Sourcy.DotNet;

namespace Build.Modules;

[DependsOn<CleanProjectsModule>]
public sealed class PackSdkModule(IOptions<PackOptions> packOptions) : Module<CommandResult>
{
    protected override async Task<CommandResult?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var changelogModule = GetModuleIfRegistered<GenerateNugetChangelogModule>();

        var changelogResult = changelogModule is null ? null : await changelogModule;
        var changelog = changelogResult?.Value ?? string.Empty;
        var outputFolder = context.Git().RootDirectory.GetFolder(packOptions.Value.OutputDirectory);

        return await context.DotNet().Pack(new DotNetPackOptions
        {
            ProjectSolution = Projects.Nice3point_Revit_Sdk.FullName,
            Configuration = Configuration.Release,
            Properties = new List<KeyValue>
            {
                ("Version", packOptions.Value.Version),
                ("PackageReleaseNotes", changelog)
            },
            OutputDirectory = outputFolder
        }, cancellationToken);
    }
}