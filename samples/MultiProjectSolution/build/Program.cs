using Build.Pipeline;
using Build.Pipeline.Compilation;
using Build.Pipeline.Packaging;
using Build.Pipeline.Publishing;
using Build.Pipeline.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = Pipeline.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddUserSecrets<Program>();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddOptionsWithValidateOnStart<BuildOptions>().Bind(builder.Configuration.GetSection(BuildOptions.ConfigurationSectionName));
builder.Services.AddSingleton<IValidateOptions<BuildOptions>, BuildOptionsValidator>();
builder.Services.AddOptionsWithValidateOnStart<BundleOptions>().Bind(builder.Configuration.GetSection(BundleOptions.ConfigurationSectionName));
builder.Services.AddSingleton<IValidateOptions<BundleOptions>, BundleOptionsValidator>();
builder.Services.AddOptionsWithValidateOnStart<InstallerOptions>().Bind(builder.Configuration.GetSection(InstallerOptions.ConfigurationSectionName));
builder.Services.AddSingleton<IValidateOptions<InstallerOptions>, InstallerOptionsValidator>();
builder.Services.AddOptions<PublishOptions>().Bind(builder.Configuration.GetSection(PublishOptions.ConfigurationSectionName));

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
