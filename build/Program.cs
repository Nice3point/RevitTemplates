using Build.Pipeline;
using Build.Pipeline.Compilation;
using Build.Pipeline.Packaging;
using Build.Pipeline.Publishing;
using Build.Pipeline.Testing;
using Build.Pipeline.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = Pipeline.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddUserSecrets<Program>();
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

builder.Services.AddOptionsWithValidateOnStart<BuildOptions>().Bind(builder.Configuration.GetSection(BuildOptions.ConfigurationSectionName));
builder.Services.AddSingleton<IValidateOptions<BuildOptions>, BuildOptionsValidator>();
builder.Services.AddOptionsWithValidateOnStart<NuGetOptions>().Bind(builder.Configuration.GetSection(NuGetOptions.ConfigurationSectionName));
builder.Services.AddSingleton<IValidateOptions<NuGetOptions>, NuGetOptionsValidator>();
builder.Services.AddOptions<PublishOptions>().Bind(builder.Configuration.GetSection(PublishOptions.ConfigurationSectionName));

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
