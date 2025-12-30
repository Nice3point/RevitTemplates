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

/// <summary>
///     Pack the SDK NuGet package.
/// </summary>
[DependsOn<CleanProjectModule>]
[DependsOn<ResolveVersioningModule>]
[DependsOn<GenerateNugetChangelogModule>]
public sealed class PackSdkModule(IOptions<BuildOptions> buildOptions) : Module<CommandResult>
{
    protected override async Task<CommandResult?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await GetModule<ResolveVersioningModule>();
        var changelogResult = await GetModule<GenerateNugetChangelogModule>();

        var versioning = versioningResult.Value!;
        var changelog = changelogResult.Value ?? string.Empty;
        var outputFolder = context.Git().RootDirectory.GetFolder(buildOptions.Value.OutputDirectory);

        return await context.DotNet().Pack(new DotNetPackOptions
        {
            ProjectSolution = Projects.Nice3point_Revit_Sdk.FullName,
            Configuration = Configuration.Release,
            Properties = new List<KeyValue>
            {
                ("VersionPrefix", versioning.VersionPrefix),
                ("VersionSuffix", versioning.VersionSuffix!),
                ("PackageReleaseNotes", changelog)
            },
            OutputDirectory = outputFolder
        }, cancellationToken);
    }
}