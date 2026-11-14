using Build.Options;
using Build.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = Pipeline.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddUserSecrets<Program>();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddOptions<BuildOptions>().Bind(builder.Configuration.GetSection(BuildOptions.ConfigurationSectionName)).ValidateDataAnnotations();
builder.Services.AddOptions<BundleOptions>().Bind(builder.Configuration.GetSection(BundleOptions.ConfigurationSectionName)).ValidateDataAnnotations();
builder.Services.AddOptions<InstallerOptions>().Bind(builder.Configuration.GetSection(InstallerOptions.ConfigurationSectionName)).ValidateDataAnnotations();
builder.Services.AddOptions<PublishOptions>().Bind(builder.Configuration.GetSection(PublishOptions.ConfigurationSectionName)).ValidateDataAnnotations();

if (args.Length == 0)
{
    builder.AddModule<CompileProjectModule>();
}

if (args.Contains("test"))
{
    builder.AddModule<TestProjectModule>();
}

if (args.Contains("pack"))
{
    builder.AddModule<CleanProjectModule>();
    builder.AddModule<CreateBundleModule>();
    builder.AddModule<CreateInstallerModule>();
}

if (args.Contains("publish"))
{
    builder.AddModule<PublishGithubModule>();
}

await builder.RunAsync();
