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
            .AddCommandLine(args)
            .AddEnvironmentVariables();
    })
    .ConfigureServices((context, collection) =>
    {
        if (args.Length == 0)
        {
            collection.AddModule<CompileProjectsModule>();
            return;
        }

        if (args.Contains("pack"))
        {
            collection.AddOptions<PackOptions>().Bind(context.Configuration.GetSection("Pack")).ValidateDataAnnotations();

            collection.AddModule<CleanProjectsModule>();
            collection.AddModule<PackSdkModule>();
            collection.AddModule<PackTemplatesModule>();
        }

        if (args.Contains("publish"))
        {
            collection.AddOptions<NuGetOptions>().Bind(context.Configuration.GetSection("NuGet")).ValidateDataAnnotations();

            collection.AddModule<GenerateChangelogModule>();
            collection.AddModule<GenerateNugetChangelogModule>();
            collection.AddModule<GenerateGitHubChangelogModule>();
            collection.AddModule<PublishNugetModule>();
            collection.AddModule<PublishGithubModule>();
        }
    })
    .ExecutePipelineAsync();