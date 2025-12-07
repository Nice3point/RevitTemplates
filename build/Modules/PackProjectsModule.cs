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
public sealed class PackProjectsModule(IOptions<PackOptions> packOptions) : Module<CommandResult>
{
    protected override async Task<CommandResult?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var changelogModule = GetModuleIfRegistered<CreatePackageChangelogModule>();

        var changelog = changelogModule is null ? null : await changelogModule;
        var outputFolder = context.Git().RootDirectory.GetFolder(packOptions.Value.OutputDirectory);

        return await context.DotNet().Pack(new DotNetPackOptions
        {
            ProjectSolution = Projects.Nice3point_Revit_Templates.FullName,
            Configuration = Configuration.Release,
            Verbosity = Verbosity.Minimal,
            Properties = new List<KeyValue>
            {
                ("Version", packOptions.Value.Version),
                ("PackageReleaseNotes", changelog is null ? string.Empty : changelog.Value)
            },
            OutputDirectory = outputFolder
        }, cancellationToken);
    }
}