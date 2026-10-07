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

builder.Services.AddOptions<BuildOptions>().Bind(builder.Configuration.GetSection(BuildOptions.ConfigurationSectionName)).ValidateDataAnnotations();
builder.Services.AddOptions<NuGetOptions>().Bind(builder.Configuration.GetSection(NuGetOptions.ConfigurationSectionName)).ValidateDataAnnotations();
builder.Services.AddOptions<PublishOptions>().Bind(builder.Configuration.GetSection(PublishOptions.ConfigurationSectionName)).ValidateDataAnnotations();

if (args.Length == 0)
{
    builder.Services.AddModule<CompileProjectModule>();
}

if (args.Contains("pack"))
{
    builder.Services.AddModule<CleanProjectModule>();
    builder.Services.AddModule<PackSdkModule>();
    builder.Services.AddModule<PackTemplatesModule>();
    builder.Services.AddModule<RestoreTemplatesReadmeModule>();
}

if (args.Contains("test"))
{
    builder.Services.AddModule<TestTemplatesModule>();
}

if (args.Contains("publish"))
{
    builder.Services.AddModule<CompileSamplesModule>();
    builder.Services.AddModule<PublishNugetModule>();
    builder.Services.AddModule<PublishGithubModule>();
}

await builder.Build().RunAsync();
