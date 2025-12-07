using System.Text.RegularExpressions;
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

[DependsOn<CleanProjectsModule>]
public sealed partial class PackTemplatesModule(IOptions<PackOptions> packOptions) : Module<CommandResult>
{
    protected override async Task<CommandResult?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var changelogModule = GetModuleIfRegistered<CreatePackageChangelogModule>();

        var changelog = changelogModule is null ? null : await changelogModule;
        var outputFolder = context.Git().RootDirectory.GetFolder(packOptions.Value.OutputDirectory);

        List<string> updatedFiles = [];

        try
        {
            updatedFiles = await SetSdkVersion(packOptions.Value.Version);
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

    private async Task<List<string>> SetSdkVersion(string version)
    {
        var modifiedFiles = new List<string>();

        var templatesProjectFile = new File(Projects.Nice3point_Revit_Templates.FullName);
        var templatesDirectory = templatesProjectFile.Folder!;

        var templateProjectFiles = templatesDirectory
            .GetFiles(path => path.Extension == ".csproj")
            .ToArray();

        var regex = SdkVersionRegex();
        var replacement = $"""
                           Project="Nice3point.Revit.Sdk/{version}"
                           """;

        foreach (var file in templateProjectFiles)
        {
            var content = await file.ReadAsync();
            if (!regex.IsMatch(content)) continue;
            
            await file.WriteAsync(regex.Replace(content, replacement));
            modifiedFiles.Add(file.Path);
        }

        return modifiedFiles;
    }

    [GeneratedRegex("""
                    Project="Nice3point\.Revit\.Sdk"
                    """)]
    private static partial Regex SdkVersionRegex();
}