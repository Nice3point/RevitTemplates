using Build.Modules;
using Build.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModularPipelines;
using ModularPipelines.Extensions;

var builder = Pipeline.CreateBuilder();

builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddUserSecrets<Program>();
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

builder.Services.Configure<BuildOptions>(builder.Configuration.GetSection("Build"));
builder.Services.Configure<NuGetOptions>(builder.Configuration.GetSection("NuGet"));
builder.Services.Configure<PublishOptions>(builder.Configuration.GetSection("Publish"));

if (args.Length == 0)
{
    builder.Services.AddModule<CompileProjectModule>();
}

if (args.Contains("pack"))
{
    builder.Services.AddModule<CleanProjectsModule>();
    builder.Services.AddModule<PackSdkModule>();
    builder.Services.AddModule<PackTemplatesModule>();
    builder.Services.AddModule<RestoreReadmeModule>();
}

if (args.Contains("publish"))
{
    builder.Services.AddModule<TestTemplatesModule>();
    builder.Services.AddModule<PublishNugetModule>();
    builder.Services.AddModule<PublishGithubModule>();
}

await builder.Build().RunAsync();