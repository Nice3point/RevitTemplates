using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Enums;
using ModularPipelines.FileSystem;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using ModularPipelines.Options;
using Shouldly;
using Sourcy.DotNet;
using File = ModularPipelines.FileSystem.File;

namespace Build.Modules;

/// <summary>
///     Create the .msi installer.
/// </summary>
[DependsOn<ResolveVersioningModule>]
[DependsOn<CompileProjectsModule>]
public sealed class CreateInstallersModule : Module<CommandResult>
{
    protected override async Task<CommandResult?> ExecuteAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.Value!;

        var wixTarget = new File(Projects.Nice3point.Revit.AddIn.FullName);
        var wixInstaller = new File(Projects.Installer.FullName);
        var wixToolFolder = await InstallWixAsync(context, cancellationToken);

        await context.DotNet().Build(new DotNetBuildOptions
        {
            ProjectSolution = wixInstaller.Path,
            Configuration = Configuration.Release,
            Properties =
            [
                ("Version", versioning.Version)
            ]
        }, cancellationToken);

        var builderFile = wixInstaller.Folder!
            .GetFolder("bin")
            .FindFile(file => file.NameWithoutExtension == wixInstaller.NameWithoutExtension && file.Extension == ".exe");

        builderFile.ShouldNotBeNull($"No installer builder was found for the project: {wixInstaller.NameWithoutExtension}");

        var targetDirectories = wixTarget.Folder!
            .GetFolder("bin")
            .GetFolders(folder => folder.Name == "publish")
            .Select(folder => folder.Path)
            .ToArray();

        targetDirectories.ShouldNotBeEmpty("No content were found to create an installer");

        return await context.Command.ExecuteCommandLineTool(new CommandLineToolOptions(builderFile.Path)
        {
            Arguments = targetDirectories,
            WorkingDirectory = context.Git().RootDirectory,
            CommandLogging = CommandLogging.Default & ~CommandLogging.Input,
            EnvironmentVariables = new Dictionary<string, string?>
            {
                { "PATH", $"{Environment.GetEnvironmentVariable("PATH")};{wixToolFolder}" }
            }
        }, cancellationToken);
    }

    /// <summary>
    ///     Installs the WiX toolset required for building installers.
    /// </summary>
    private static async Task<Folder> InstallWixAsync(IPipelineContext context, CancellationToken cancellationToken)
    {
        var wixToolFolder = context.FileSystem.CreateTemporaryFolder();
#if (_needToUpdatePipelineVersion)
        await context.DotNet().Tool.Install(new DotNetToolInstallOptions("wix")
        {
            ToolPath = wixToolFolder.Path
        }, cancellationToken);
#endif
        await context.Command.ExecuteCommandLineTool(new CommandLineToolOptions("dotnet")
        {
            Arguments =
            [
                "tool",
                "install",
                "--tool-path", wixToolFolder.Path,
                "wix"
            ]
        }, cancellationToken);

        return wixToolFolder;
    }
}