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
#if (includeBundle)
builder.Services.AddOptions<BundleOptions>().Bind(builder.Configuration.GetSection("Bundle")).ValidateDataAnnotations();
#endif
#if (isGitHubCi)
builder.Services.AddOptions<PublishOptions>().Bind(builder.Configuration.GetSection("Publish")).ValidateDataAnnotations();
#endif

if (args.Length == 0)
{
    builder.Services.AddModule<CompileProjectModule>();
}
#if (includeTests)

if (args.Contains("test"))
{
    builder.Services.AddModule<TestProjectModule>();
}
#endif
#if (hasArtifacts)

if (args.Contains("pack"))
{
    builder.Services.AddModule<CleanProjectModule>();
#if (includeBundle)
    builder.Services.AddModule<CreateBundleModule>();
#endif
#if (includeInstaller)
    builder.Services.AddModule<CreateInstallerModule>();
#endif
}
#endif
#if (isGitHubCi)
if (args.Contains("publish"))
{
    builder.Services.AddModule<PublishGithubModule>();
}
#endif

await builder.Build().RunAsync();