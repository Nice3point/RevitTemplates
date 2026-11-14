using Build.Options;
using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Options;
using ModularPipelines.FileSystem;
using Sourcy.DotNet;

namespace Build.Pipeline;

/// <summary>
///     Represents the pipeline step that packs the SDK NuGet package.
/// </summary>
[DependsOn<ResolveVersioningModule>]
[DependsOn<UpdateTemplatesReadmeModule>(Optional = true)]
[DependsOn<CleanProjectModule>(Optional = true)]
[DependsOn<GenerateNugetChangelogModule>(Optional = true)]
public sealed class PackSdkModule(IOptions<BuildOptions> buildOptions) : Module
{
    protected override async Task ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var changelogModule = context.GetModuleIfRegistered<GenerateNugetChangelogModule>();

        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var changelogResult = changelogModule is null ? null : await changelogModule;

        var versioning = versioningResult.Value;
        var changelog = changelogResult?.ValueOrDefault ?? string.Empty;
        var repository = await context.Tools.Git.Information.GetRequiredInfoAsync(cancellationToken);
        var outputFolder = repository.Root.GetFolder(buildOptions.Value.OutputDirectory);
        var targetProject = new FilePath(Projects.Nice3point_Revit_Sdk.FullName);

        await context.Tools.DotNet.PackAsync(new DotNetPackOptions
        {
            ProjectSolution = targetProject.Path,
            Configuration = "Release",
            Output = outputFolder.Path,
            Properties =
            [
                ("VersionPrefix", versioning.VersionPrefix),
                ("VersionSuffix", versioning.VersionSuffix ?? string.Empty),
                ("PackageReleaseNotes", changelog)
            ]
        }, cancellationToken: cancellationToken);

        context.Summary.KeyValue("Artifacts", "Sdk", outputFolder.FindFile(file => file.Name.StartsWith(targetProject.NameWithoutExtension))!.Path);
    }
}
