using Build.Pipeline;
using Build.Pipeline.Compilation;
#if (hasArtifacts)
using Build.Pipeline.Packaging;
#endif
#if (isGitHubCi)
using Build.Pipeline.Publishing;
#endif
#if (includeTests)
using Build.Pipeline.Testing;
#endif
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
#if (hasArtifacts)
using Microsoft.Extensions.Options;
#endif

var builder = Pipeline.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddUserSecrets<Program>();
builder.Configuration.AddEnvironmentVariables();

#if (hasArtifacts)
builder.Services.AddOptionsWithValidateOnStart<BuildOptions>().Bind(builder.Configuration.GetSection(BuildOptions.ConfigurationSectionName));
builder.Services.AddSingleton<IValidateOptions<BuildOptions>, BuildOptionsValidator>();
#else
builder.Services.AddOptions<BuildOptions>().Bind(builder.Configuration.GetSection(BuildOptions.ConfigurationSectionName));
#endif
#if (includeBundle)
builder.Services.AddOptionsWithValidateOnStart<BundleOptions>().Bind(builder.Configuration.GetSection(BundleOptions.ConfigurationSectionName));
builder.Services.AddSingleton<IValidateOptions<BundleOptions>, BundleOptionsValidator>();
#endif
#if (includeInstaller)
builder.Services.AddOptionsWithValidateOnStart<InstallerOptions>().Bind(builder.Configuration.GetSection(InstallerOptions.ConfigurationSectionName));
builder.Services.AddSingleton<IValidateOptions<InstallerOptions>, InstallerOptionsValidator>();
#endif
#if (isGitHubCi)
builder.Services.AddOptions<PublishOptions>().Bind(builder.Configuration.GetSection(PublishOptions.ConfigurationSectionName));
#endif

if (args.Length == 0 || args.Contains("compile"))
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
