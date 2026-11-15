using Build.Pipeline.Versioning;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Options;
using Sourcy.DotNet;

namespace Build.Pipeline.Compilation;

/// <summary>
///     Represents the pipeline step that compiles the solution.
/// </summary>
[DependsOn<ResolveVersioningModule>]
public sealed class CompileProjectModule : Module
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.Value;

        await context.Tools.DotNet.BuildAsync(new DotNetBuildOptions
        {
            ProjectSolution = Solutions.Nice3point_Revit_Templates.FullName,
            Configuration = "Release",
            Properties =
            [
                ("VersionPrefix", versioning.VersionPrefix),
                ("VersionSuffix", versioning.VersionSuffix ?? string.Empty)
            ]
        }, cancellationToken: cancellationToken);
    }
}
