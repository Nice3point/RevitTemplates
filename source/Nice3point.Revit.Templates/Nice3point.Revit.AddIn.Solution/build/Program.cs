using Build.Modules;
using Build.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModularPipelines.Extensions;
using ModularPipelines.Host;

await PipelineHostBuilder.Create()
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.AddJsonFile("appsettings.json")
            .AddEnvironmentVariables();
    })
    .ConfigureServices((context, collection) =>
    {
        collection.AddOptions<BuildOptions>().Bind(context.Configuration.GetSection("Build")).ValidateDataAnnotations();

        collection.AddModule<ResolveConfigurationsModule>();
        collection.AddModule<ResolveVersioningModule>();
        collection.AddModule<CleanProjectsModule>();
        collection.AddModule<CompileProjectsModule>();
#if (HasArtifacts)

        if (args.Contains("pack"))
        {
            collection.AddOptions<BundleOptions>().Bind(context.Configuration.GetSection("Bundle")).ValidateDataAnnotations();

#if (bundle)
            collection.AddModule<CreateBundleModule>();
#endif
#if (installer)
            collection.AddModule<CreateInstallersModule>();
#endif
        }
#endif
#if (GitHubPipeline && HasArtifacts)

        if (args.Contains("publish"))
        {
            collection.AddOptions<PublishOptions>().Bind(context.Configuration.GetSection("Publish")).ValidateDataAnnotations();

            collection.AddModule<GenerateChangelogModule>();
            collection.AddModule<GenerateGitHubChangelogModule>();
            collection.AddModule<PublishGithubModule>();
        }
#endif
    })
    .ExecutePipelineAsync();