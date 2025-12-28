using Build.Options;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Git.Options;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using Sourcy.DotNet;
using File = ModularPipelines.FileSystem.File;

namespace Build.Modules;

[DependsOn<CleanProjectModule>]
public sealed class PackTemplatesModule(IOptions<BuildOptions> buildOptions) : Module<CommandResult>
{
    protected override async Task<CommandResult?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var changelogModule = GetModuleIfRegistered<GenerateNugetChangelogModule>();

        var changelogResult = changelogModule is null ? null : await changelogModule;
        var changelog = changelogResult?.Value ?? string.Empty;
        var outputFolder = context.Git().RootDirectory.GetFolder(buildOptions.Value.OutputDirectory);

        List<string> updatedFiles = [];

        try
        {
            updatedFiles = await SetSdkVersionAsync(buildOptions.Value.Version, cancellationToken);
            return await context.DotNet().Pack(new DotNetPackOptions
            {
                ProjectSolution = Projects.Nice3point_Revit_Templates.FullName,
                Configuration = Configuration.Release,
                Properties = new List<KeyValue>
                {
                    ("Version", buildOptions.Value.Version),
                    ("PackageReleaseNotes", changelog)
                },
                OutputDirectory = outputFolder
            }, cancellationToken);
        }
        finally
        {
            if (updatedFiles.Count > 0)
            {
                await context.Git().Commands.Restore(new GitRestoreOptions
                {
                    Arguments = updatedFiles
                }, cancellationToken);
            }
        }
    }

    private static async Task<List<string>> SetSdkVersionAsync(string version, CancellationToken cancellationToken)
    {
        var modifiedFiles = new List<string>();

        var templatesProjectFile = new File(Projects.Nice3point_Revit_Templates.FullName);
        var templatesDirectory = templatesProjectFile.Folder!;

        var templateProjectFiles = templatesDirectory
            .GetFiles(path => path.Extension == ".csproj")
            .ToArray();

        foreach (var file in templateProjectFiles)
        {
            var content = await file.ReadAsync(cancellationToken);

            await file.WriteAsync(content.Replace(
                """
                <Project Sdk="Nice3point.Revit.Sdk">
                """,
                $"""
                 <Project Sdk="Nice3point.Revit.Sdk/{version}">
                 """), cancellationToken);
            modifiedFiles.Add(file.Path);
        }

        return modifiedFiles;
    }
}