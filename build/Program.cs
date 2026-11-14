using Build.Options;
using Build.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = Pipeline.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddUserSecrets<Program>();
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

builder.Services.AddOptions<BuildOptions>().Bind(builder.Configuration.GetSection(BuildOptions.ConfigurationSectionName)).ValidateDataAnnotations();
builder.Services.AddOptions<NuGetOptions>().Bind(builder.Configuration.GetSection(NuGetOptions.ConfigurationSectionName)).ValidateDataAnnotations();
builder.Services.AddOptions<PublishOptions>().Bind(builder.Configuration.GetSection(PublishOptions.ConfigurationSectionName)).ValidateDataAnnotations();

if (args.Length == 0)
{
    builder.AddModule<CompileProjectModule>();
}

if (args.Contains("pack"))
{
    builder.AddModule<CleanProjectModule>();
    builder.AddModule<PackSdkModule>();
    builder.AddModule<PackTemplatesModule>();
    builder.AddModule<RestoreTemplatesReadmeModule>();
}

if (args.Contains("test"))
{
    builder.AddModule<TestTemplatesModule>();
}

if (args.Contains("publish"))
{
    builder.AddModule<CompileSamplesModule>();
    builder.AddModule<PublishNugetModule>();
    builder.AddModule<PublishGithubModule>();
}

await builder.RunAsync();
