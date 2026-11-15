using Build.Pipeline.Compilation;
using Build.Pipeline.Publishing;
using Build.Pipeline.Versioning;
using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Options;
using ModularPipelines.FileSystem;
using ModularPipelines.Git.Options;
using Sourcy.DotNet;

namespace Build.Pipeline.Packaging;

/// <summary>
///     Represents the pipeline step that packs the templates NuGet package.
/// </summary>
/// <remarks>The step pins the template projects to the SDK of the same version while it packs, and restores the project files from Git afterward.</remarks>
[DependsOn<ResolveVersioningModule>]
[DependsOn<UpdateTemplatesReadmeModule>(Optional = true)]
[DependsOn<CleanProjectModule>(Optional = true)]
[DependsOn<GenerateNugetChangelogModule>(Optional = true)]
public sealed class PackTemplatesModule(IOptions<BuildOptions> buildOptions) : Module
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var changelogModule = context.GetModuleIfRegistered<GenerateNugetChangelogModule>();

        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var changelogResult = changelogModule is null ? null : await changelogModule;

        var versioning = versioningResult.Value;
        var changelog = changelogResult?.ValueOrDefault ?? string.Empty;
        var repository = await context.Tools.Git.Information.GetRequiredInfoAsync(cancellationToken);
        var outputFolder = repository.Root.GetFolder(buildOptions.Value.OutputDirectory);
        var targetProject = new FilePath(Projects.Nice3point_Revit_Templates.FullName);

        List<string> updatedFiles = [];

        try
        {
            updatedFiles = await SetSdkVersionAsync(versioning.Version, cancellationToken);
            await context.Tools.DotNet.PackAsync(new DotNetPackOptions
            {
                ProjectSolution = targetProject.Path,
                Configuration = "Release",
                Properties =
                [
                    ("VersionPrefix", versioning.VersionPrefix),
                    ("VersionSuffix", versioning.VersionSuffix ?? string.Empty),
                    ("PackageReleaseNotes", changelog)
                ],
                Output = outputFolder.Path
            }, cancellationToken: cancellationToken);

            context.Summary.KeyValue("Artifacts", "Templates", outputFolder.FindFile(file => file.Name.StartsWith(targetProject.NameWithoutExtension))!.Path);
        }
        finally
        {
            if (updatedFiles.Count > 0)
            {
                await context.Tools.Git.Commands.WorkingTree.RestoreAsync(new GitRestoreOptions
                {
                    Arguments = updatedFiles
                }, cancellationToken: cancellationToken);
            }
        }
    }

    /// <summary>
    ///     Pins the template projects to the SDK of the specified version.
    /// </summary>
    /// <returns>The paths of the modified project files.</returns>
    private static async Task<List<string>> SetSdkVersionAsync(string version, CancellationToken cancellationToken)
    {
        var modifiedFiles = new List<string>();

        var templatesProjectFile = new FilePath(Projects.Nice3point_Revit_Templates.FullName);
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
