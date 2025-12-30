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
        collection.AddOptions<BuildOptions>().Bind(context.Configuration.GetSection("Build")).ValidateDataAnnotations();

        collection.AddModule<ResolveVersioningModule>();

        if (args.Length == 0)
        {
            collection.AddModule<CompileProjectModule>();
            return;
        }

        if (args.Contains("pack"))
        {
            collection.AddModule<CleanProjectModule>();
            collection.AddModule<PackSdkModule>();
            collection.AddModule<PackTemplatesModule>();
            collection.AddModule<GenerateChangelogModule>();
            collection.AddModule<GenerateNugetChangelogModule>();
            collection.AddModule<UpdateTemplatesReadmeModule>();
            collection.AddModule<RestoreTemplatesReadmeModule>();
        }

        if (args.Contains("publish"))
        {
            collection.AddOptions<NuGetOptions>().Bind(context.Configuration.GetSection("NuGet")).ValidateDataAnnotations();

            collection.AddModule<GenerateGitHubChangelogModule>();
            collection.AddModule<PublishNugetModule>();
            collection.AddModule<PublishGithubModule>();
        }
    })
    .ExecutePipelineAsync();