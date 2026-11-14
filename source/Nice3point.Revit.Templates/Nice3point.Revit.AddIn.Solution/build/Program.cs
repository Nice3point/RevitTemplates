using Build.Options;
using Build.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = Pipeline.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddUserSecrets<Program>();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddOptions<BuildOptions>().Bind(builder.Configuration.GetSection(BuildOptions.ConfigurationSectionName)).ValidateDataAnnotations();
#if (includeBundle)
builder.Services.AddOptions<BundleOptions>().Bind(builder.Configuration.GetSection(BundleOptions.ConfigurationSectionName)).ValidateDataAnnotations();
#endif
#if (includeInstaller)
builder.Services.AddOptions<InstallerOptions>().Bind(builder.Configuration.GetSection(InstallerOptions.ConfigurationSectionName)).ValidateDataAnnotations();
#endif
#if (isGitHubCi)
builder.Services.AddOptions<PublishOptions>().Bind(builder.Configuration.GetSection(PublishOptions.ConfigurationSectionName)).ValidateDataAnnotations();
#endif

if (args.Length == 0)
{
    builder.AddModule<CompileProjectModule>();
}
#if (includeTests)

if (args.Contains("test"))
{
    builder.AddModule<TestProjectModule>();
}
#endif
#if (hasArtifacts)

if (args.Contains("pack"))
{
    builder.AddModule<CleanProjectModule>();
#if (includeBundle)
    builder.AddModule<CreateBundleModule>();
#endif
#if (includeInstaller)
    builder.AddModule<CreateInstallerModule>();
#endif
}
#endif
#if (isGitHubCi)

if (args.Contains("publish"))
{
    builder.AddModule<PublishGithubModule>();
}
#endif

await builder.RunAsync();
