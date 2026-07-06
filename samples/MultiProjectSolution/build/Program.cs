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

builder.Services.AddOptions<BuildOptions>().Bind(builder.Configuration.GetSection("Build")).ValidateDataAnnotations();
builder.Services.AddOptions<BundleOptions>().Bind(builder.Configuration.GetSection("Bundle")).ValidateDataAnnotations();
builder.Services.AddOptions<PublishOptions>().Bind(builder.Configuration.GetSection("Publish"));

if (args.Length == 0)
{
    builder.Services.AddModule<CompileProjectModule>();
}

if (args.Contains("test"))
{
    builder.Services.AddModule<TestProjectModule>();
}

if (args.Contains("pack"))
{
    builder.Services.AddModule<CleanProjectModule>();
    builder.Services.AddModule<CreateBundleModule>();
    builder.Services.AddModule<CreateInstallerModule>();
}

if (args.Contains("publish"))
{
    builder.Services.AddModule<PublishGithubModule>();
}

await builder.Build().RunAsync();