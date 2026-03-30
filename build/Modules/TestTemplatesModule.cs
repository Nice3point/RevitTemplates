using System.Text.RegularExpressions;
using Build.Options;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.FileSystem;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;
using ModularPipelines.Options;
using Shouldly;
using Sourcy.DotNet;
using File = ModularPipelines.FileSystem.File;

namespace Build.Modules;

/// <summary>
///     Create and build projects from templates to verify they work correctly.
/// </summary>
[DependsOn<PackTemplatesModule>]
[DependsOn<PackSdkModule>]
public sealed partial class TestTemplatesModule(IOptions<BuildOptions> buildOptions) : Module
{
    protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var outputFolder = context.Git().RootDirectory.GetFolder(buildOptions.Value.OutputDirectory);
        var targetProject = new File(Projects.Nice3point_Revit_Templates.FullName);
        var templatesPackage = outputFolder.GetFiles(file => file.Name.StartsWith(targetProject.NameWithoutExtension) && file.Extension == ".nupkg").First();
        var samplesFolder = outputFolder.CreateFolder("generated");

        await UninstallTemplatesAsync(context, targetProject.NameWithoutExtension, cancellationToken);
        await InstallTemplatesAsync(context, templatesPackage.Path, cancellationToken);

        try
        {
            var matrix = GenerateMatrix();
            foreach (var templateMetadata in matrix)
            {
                var projectFolder = await GenerateProjectAsync(context, templateMetadata.Template, templateMetadata.Options, samplesFolder, cancellationToken);
                foreach (var subTemplate in templateMetadata.SubTemplates)
                {
                    await GenerateSubProjectAsync(context, subTemplate, projectFolder, cancellationToken);
                }

                await GenerateNuGetConfigAsync(projectFolder, outputFolder, cancellationToken);
                await CompileGeneratedProjectsAsync(context, projectFolder, cancellationToken);
            }
        }
        finally
        {
            await UninstallTemplatesAsync(context, targetProject.NameWithoutExtension, cancellationToken);
            await samplesFolder.DeleteAsync(cancellationToken);
        }
    }

    private static async Task InstallTemplatesAsync(IModuleContext context, string templatesPackage, CancellationToken cancellationToken)
    {
        await context.DotNet().New.Execute(new DotNetNewOptions
        {
            Arguments = ["install", templatesPackage]
        }, cancellationToken: cancellationToken);
    }

    private static async Task UninstallTemplatesAsync(IModuleContext context, string templatesPackage, CancellationToken cancellationToken)
    {
        await context.DotNet().New.Execute(new DotNetNewOptions
            {
                Arguments = ["uninstall", templatesPackage]
            },
            new CommandExecutionOptions
            {
                ThrowOnNonZeroExitCode = false
            }, cancellationToken: cancellationToken);
    }

    private static async Task<Folder> GenerateProjectAsync(IModuleContext context, string template, Dictionary<string, string> options, Folder samplesFolder, CancellationToken cancellationToken)
    {
        var projectFolder = samplesFolder.CreateFolder($"{template}-{Guid.NewGuid():N}");
        await context.DotNet().New.Execute(new DotNetNewOptions
        {
            TemplateShortName = template,
            Name = "Generated",
            Output = projectFolder,
            Arguments = options.SelectMany(pair => new[] { $"--{pair.Key}", pair.Value })
        }, cancellationToken: cancellationToken);
        return projectFolder;
    }

    private static async Task GenerateSubProjectAsync(IModuleContext context, string template, Folder projectFolder, CancellationToken cancellationToken)
    {
        await context.DotNet().New.Execute(new DotNetNewOptions
        {
            TemplateShortName = template,
            Name = "Generated",
            Output = projectFolder.GetFolder("source")
        }, cancellationToken: cancellationToken);
    }

    private static async Task CompileGeneratedProjectsAsync(IModuleContext context, Folder projectFolder, CancellationToken cancellationToken)
    {
        var projectFiles = projectFolder.GetFiles(file => file.Extension is ".csproj").ToArray();
        projectFiles.Length.ShouldBeGreaterThan(0);

        foreach (var projectFile in projectFiles)
        {
            var projectContent = await projectFile.ReadAsync(cancellationToken);
            var hasSdk = projectContent.Contains("Nice3point.Revit.Sdk");
            var configuration = hasSdk ? ParseProjectConfigurations(projectContent)[0] : "Release";

            await context.DotNet().Build(new DotNetBuildOptions
            {
                ProjectSolution = projectFile.Path,
                Configuration = configuration
            }, cancellationToken: cancellationToken);
        }
    }

    private static string[] ParseProjectConfigurations(string projectContent)
    {
        var configurationsMatch = ConfigurationsRegex().Matches(projectContent);
        return configurationsMatch
            .Select(match => match.Groups[1].Value)
            .SelectMany(config => config.Split(';'))
            .Where(config => config.StartsWith("Release."))
            .ToArray();
    }

    private static async Task GenerateNuGetConfigAsync(Folder projectFolder, Folder outputFolder, CancellationToken cancellationToken)
    {
        var nugetConfigPath = projectFolder.GetFile("NuGet.config");
        await nugetConfigPath.WriteAsync($"""
                                          <?xml version="1.0" encoding="utf-8"?>
                                          <configuration>
                                            <packageSources>
                                              <add key="local" value="{outputFolder.Path}" />
                                            </packageSources>
                                          </configuration>
                                          """, cancellationToken);
    }

    private static List<TemplateMetadata> GenerateMatrix()
    {
        var matrix = new List<TemplateMetadata>();

        string[] addInManifestTypes = ["application", "dbApplication", "command"];
        string[] addInDiModes = ["disabled", "container", "hosting"];
        string[] boolOptions = ["true", "false"];

        // Nice3point.Revit.AddIn
        foreach (var type in addInManifestTypes)
        {
            foreach (var di in addInDiModes)
            {
                foreach (var wpf in boolOptions)
                {
                    foreach (var logging in boolOptions)
                    {
                        matrix.Add(new TemplateMetadata
                        {
                            Template = "revit-addin",
                            Options = new Dictionary<string, string>
                            {
                                { "addin", type },
                                { "di", di },
                                { "wpf", wpf },
                                { "logger", logging }
                            }
                        });
                    }
                }
            }
        }
        
        // Nice3point.Revit.AddIn.Application
        foreach (var type in addInManifestTypes)
        {
            foreach (var di in addInDiModes)
            {
                foreach (var logging in boolOptions)
                {
                    matrix.Add(new TemplateMetadata
                    {
                        Template = "revit-addin-application",
                        Options = new Dictionary<string, string>
                        {
                            { "addin", type },
                            { "di", di },
                            { "logger", logging }
                        }
                    });
                }
            }
        }
        
        // Nice3point.Revit.AddIn.Module
        foreach (var wpf in boolOptions)
        {
            matrix.Add(new TemplateMetadata
            {
                Template = "revit-addin-module",
                Options = new Dictionary<string, string>
                {
                    { "wpf", wpf }
                }
            });
        }

        // Nice3point.Revit.AddIn.Solution
        string[] pipelines = ["github", "azure", "disabled"];
        foreach (var pipeline in pipelines)
        {
            foreach (var installer in boolOptions)
            {
                foreach (var bundle in boolOptions)
                {
                    matrix.Add(new TemplateMetadata
                    {
                        Template = "revit-addin-sln",
                        Options = new Dictionary<string, string>
                        {
                            { "pipeline", pipeline },
                            { "installer", installer },
                            { "bundle", bundle }
                        },
                        SubTemplates = ["revit-addin-application"]
                    });
                }
            }
        }

        // Nice3point.Benchmark.Revit
        matrix.Add(new TemplateMetadata
        {
            Template = "revit-benchmark"
        });

        // Nice3point.Unit.Revit
        matrix.Add(new TemplateMetadata
        {
            Template = "revit-tunit"
        });

        return matrix;
    }

    private sealed record TemplateMetadata
    {
        public required string Template { get; init; }
        public Dictionary<string, string> Options { get; init; } = [];
        public string[] SubTemplates { get; init; } = [];
    }

    [GeneratedRegex("<Configurations>(.*?)</Configurations>")]
    private static partial Regex ConfigurationsRegex();
}