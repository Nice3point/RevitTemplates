#if (hasArtifacts)
using Build.Options;
using Microsoft.Extensions.Options;
#endif
using ModularPipelines.Attributes;
using ModularPipelines.Conditions;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;
using Sourcy.DotNet;

namespace Build.Modules;

/// <summary>
#if (hasArtifacts)
///     Represents the pipeline step that cleans the build output of the projects and the artifact directory.
#else
///     Represents the pipeline step that cleans the build output of the projects.
#endif
/// </summary>
[SkipIf<IsCI>]
#if (hasArtifacts)
public sealed class CleanProjectModule(IOptions<BuildOptions> buildOptions) : SyncModule
#else
public sealed class CleanProjectModule : SyncModule
#endif
{
    protected override void ExecuteModule(IModuleContext context, CancellationToken cancellationToken)
    {
        var rootDirectory = context.Git().RootDirectory;
#if (hasArtifacts)
        var outputDirectory = rootDirectory.GetFolder(buildOptions.Value.OutputDirectory);
#endif
        var buildOutputDirectories = rootDirectory
            .GetFolders(folder => folder.Name is "bin" or "obj")
            .Where(folder => folder.Parent != Projects.Build.Directory);

        foreach (var buildFolder in buildOutputDirectories)
        {
            buildFolder.Clean();
        }
#if (hasArtifacts)

        if (outputDirectory.Exists)
        {
            outputDirectory.Clean();
        }
#endif
    }
}
