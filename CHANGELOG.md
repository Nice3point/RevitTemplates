# 6.3.0

Logging moves to `Microsoft.Extensions.Logging`, the code shared by an application and its modules moves to the new service defaults template, and the installer builds from a manifest.
In existing projects, update the SDK version and follow the [migration guide](#migration-guide).

## Templates

### Microsoft.Extensions.Logging

Add-ins log through `Microsoft.Extensions.Logging` with source-generated `LoggerMessage` methods. The Serilog packages are no longer referenced.
The logging option is available when dependency injection is enabled, and one call registers the providers:

```c#
builder.AddLoggingDefaults();
```

```c#
public sealed partial class ProjectService(ILogger<ProjectService> logger)
{
    public void Save(Document document)
    {
        document.Save();
        LogProjectSaved(logger, document.Title);
    }

    [LoggerMessage(LogLevel.Information, "The project {Title} is saved.")]
    private static partial void LogProjectSaved(ILogger<ProjectService> logger, string title);
}
```

- Records with the `Error` level and above are written to the Revit journal through the `Nice3point.Revit.Logging` provider. With hosting, the `Logging:RevitJournal:LogLevel` configuration section changes the level.
- Unhandled `AppDomain` exceptions are written to the log with the `Critical` level.
- The `Configuration` folder is replaced by `Logging` and `Diagnostics`.

### Service defaults template

The new `revit-servicedefaults` template creates a project for the service configuration that an application and its modules share, on the model of the [.NET Aspire service defaults](https://learn.microsoft.com/dotnet/aspire/fundamentals/service-defaults).
The template provides logging and diagnostics, and the project is the place for every other shared registration: serialization, HTTP clients, options.
`revit-addin-application` now creates the entry point alone, and the application host applies the shared defaults with one call:

```shell
dotnet new revit-addin-application -n RevitAddIn --di hosting
dotnet new revit-servicedefaults -n RevitAddIn.ServiceDefaults
```

```c#
builder.AddServiceDefaults();
```

### Installer

The installer project moved from the `install` folder to `installer` and builds the MSI packages from a manifest that the build writes.

- The `.addin` manifests are installed after the assemblies they reference. An interrupted installation no longer registers an add-in whose assemblies are missing.
- The upgrade code is configured in the `Installer` section of `build/appsettings.json`:

  ```json
  "Installer": {
    "UpgradeCode": "CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC"
  }
  ```

- The product name of the packages comes from the add-in project.
- The packages no longer contain `.pdb` files.

### Tests and benchmarks

- The test project is adapted to the latest `Nice3point.TUnit.Revit`, which runs test hooks on the Revit thread. `[HookExecutor<RevitThreadExecutor>]` and `TestsConfiguration.cs` are removed.
- The test template contains a TUnit assertion.
- The benchmark project runs through `BenchmarkSwitcher`. Command-line arguments select the benchmarks to run: `dotnet run -c Release.R26 -- --filter '*'`.
- The benchmark class is marked `[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]`, and the IDE no longer reports the benchmarks as unused.

### Generated code

- Entry points, views, view models, host services, and build options have XML documentation.
- The solution contains the code style in `.editorconfig` and the LF line-ending policy in `.gitattributes`.
- Generated files use LF line endings, end with a newline, and follow the code style of the solution.
- The solution pins the .NET SDK to the 10.0.1xx feature band.
- The `Models` folder is no longer created.
- The host of an add-in sets `IHostEnvironment.ApplicationName` to the project name.
- ILRepack, Polyfill, and `JetBrains.Annotations` are private assets of the generated projects.

### Build

- Every options type declares its configuration section in `ConfigurationSectionName`, and `Program.cs` binds the options through it.
- Build modules log through source-generated `LoggerMessage` methods.
- Build and installer errors state the problem, the cause, and the action that resolves it:

  ```text
  Cannot create the bundle. No publish output was found in 'source/RevitAddIn/bin'. Set 'PublishAddin' to 'true' in the add-in project.
  ```

- A solution without a bundle and an installer compiles `BuildOptions`.
- A version with a hyphen in the prerelease label keeps the whole label in `VersionSuffix`.
- The release of `1.0.0` no longer takes the release notes from the `1.0.0-rc.1` entry of the changelog.
- Release assets are uploaded as `application/octet-stream`, and the asset files are closed after the upload.
- The installer step removes the temporary WiX toolset after the build.
- The `test` step no longer disables ILRepack.

### Samples

- The sample solution keeps logging, diagnostics, and serialization in a `ServiceDefaults` project, and serializes through a source-generated context.
- The Extensible Storage sample stores its data through a schema definition and a typed context.

### Dependencies

- Updated dependencies.

## SDK

- ILRepack merges the published add-in. The `bin` directory keeps the original assemblies for the projects that reference the add-in, such as a test project.
- Repacking runs only when `PublishAddin` or `DeployAddin` is enabled.
- Updated dependencies.

## Breaking changes

- Serilog is replaced by `Microsoft.Extensions.Logging`.
- `revit-addin-application` no longer has the logging option. Logging comes from a `revit-servicedefaults` project.
- The installer project moved from `install` to `installer`, and the upgrade code moved to `build/appsettings.json`.
- Test hooks no longer use `RevitThreadExecutor`.
- Repacking requires `PublishAddin` or `DeployAddin`.

## Migration guide

### Add-in

1. Delete the `Configuration` folder from the add-in project.
2. Create a temporary project with version 6.3.0 and the same options, and copy the `Logging` and `Diagnostics` folders to the add-in project.
3. Replace the Serilog registration in `Host.cs`:

   ```c#
   // Before, hosting
   builder.Logging.ClearProviders();
   builder.Logging.AddSerilog();
   builder.ConfigureHosting();

   // After, hosting
   builder.AddLoggingDefaults();

   // Before, container
   services.AddSerilog();
   _serviceProvider = services.BuildServiceProvider();

   // After, container
   services.AddLoggingDefaults();
   _serviceProvider = services.BuildServiceProvider();
   _serviceProvider.GetRequiredService<AppDomainExceptionsHandler>().LogExceptions();
   ```

4. Remove the `Serilog`, `Serilog.Sinks.Debug`, and `Serilog.Extensions.Hosting` package references.
5. With hosting, Serilog was a provider of `Microsoft.Extensions.Logging`, and the classes already receive `ILogger<T>`. With the container option, replace the injected `Serilog.ILogger` with `ILogger<T>`:

   ```c#
   // Before
   public class ProjectService(Serilog.ILogger logger)
   {
       public void Save() => logger.Information("Message");
   }

   // After
   public partial class ProjectService(ILogger<ProjectService> logger)
   {
       public void Save() => LogMessage(logger);

       [LoggerMessage(LogLevel.Information, "Message")]
       private static partial void LogMessage(ILogger<ProjectService> logger);
   }
   ```

### Solution

1. Delete the `install` folder and `build/Modules/CreateInstallerModule.cs`.
2. Create a temporary solution with version 6.3.0, and copy the `installer` folder, `build/Modules/CreateInstallerModule.cs`, and `build/Options/InstallerOptions.cs` to the solution.
3. Replace the `install` project with `installer/Installer.csproj` in the solution file.
4. Register the options in `build/Program.cs`:

   ```c#
   builder.Services.AddOptions<InstallerOptions>().Bind(builder.Configuration.GetSection(InstallerOptions.ConfigurationSectionName)).ValidateDataAnnotations();
   ```

5. Add the `Installer` section to `build/appsettings.json`, and set `UpgradeCode` to the GUID that `install/Installer.cs` passed to `Project.GUID`. A new GUID makes the next release install side by side with the previous one.

### Tests

1. Update `Nice3point.TUnit.Revit` to the latest version.
2. Delete `TestsConfiguration.cs` and remove the executor from the test hooks:

   ```c#
   // Before
   [Before(Test)]
   [HookExecutor<RevitThreadExecutor>]
   public void SeedModel()

   // After
   [Before(Test)]
   public void SeedModel()
   ```

### SDK

A project that repacks its assemblies enables `PublishAddin` or `DeployAddin`:

```xml
<PropertyGroup>
    <PublishAddin>true</PublishAddin>
</PropertyGroup>
```

# 6.2.3

Minor fixes and polishing. In existing projects, update the SDK version.

## SDK

- Projects with a single configuration no longer fail with a missing `Configurations` parameter.
- Repacking on a non-Windows machine fails with an explanation instead of an obscure tool error.
- Repacking no longer passes your own assembly into the merge twice.
- Custom configuration names like `Staging.R26` now repack with debug behaviour instead of being silently skipped.
- Publish and deploy skip unchanged files, an incremental build no longer rewrites the whole add-in folder.
- Updated dependencies.

## Templates

- Missing required values in `appsettings.json` now fail the build instead of being ignored.
- `dotnet new` no longer reports an unsupported action after scaffolding a project.
- The generated readme documents the `tests` folder and hides the Azure release instructions when there is no Azure pipeline.
- The sample solution opens its modal window with `ShowDialog()` by @kjs-chang in https://github.com/Nice3point/RevitTemplates/pull/167.
- Documentation moved to the [Wiki](https://github.com/Nice3point/RevitTemplates/wiki) and is published automatically, all links updated.
- Updated dependencies.

# 6.2.2

## Templates

- Update TUnit to 1.44.0
- Update ModularPipelines to 3.2.8
- Update dependency Polyfill to 10.5.1
- Update WixSharp to 2.14.0
- Update dotnet to 10.0.203
- Update Microsoft monorepo to 10.0.7
- Remove extra space for some conditions

# 6.2.1

## Templates

- Accept WiX v7 EULA

# 6.2.0

## Templates

- Revit 2027 support
- Support for Async External Command/Application when using Hosting.
- Removed Revit 2022 from default configurations (can be added manually).
- Configured fluent diagnozers for Benchmark tempalte
- Pinned version for Wix extensions https://github.com/oleg-shilo/wixsharp/issues/1900
- Disabled IlRepack for CI|CD testing

## SDK

- Added implicit `Nice3point.Revit.Extensions.UI` using when `RevitAPIUI.dll` is referenced.

# 6.1.2

- Fix bundle Contents folder name https://github.com/lookup-foundation/RevitLookup/pull/360

# 6.1.1

## Templates

- Fix missing `await using` statement in **Build** project. https://github.com/Nice3point/RevitTemplates/issues/134

# 6.1.0

## SDK

- Upgraded target frameworks to .NET 10.0.
- Fixed targets execution sequence to avoid conflicts with Microsoft.NET.Sdk.

## Templates

- Migrated build system to `ModularPipelines` v3. This major upgrade has a lot of improvements, better summary output, and refactored module logic.
- Added support for semantic version handling in the Installer.
- Added support for solution testing configuration https://github.com/Nice3point/RevitTemplates/issues/118.
- Added support for template name forms, allowing illegal characters for project name https://github.com/Nice3point/RevitTemplates/issues/117.
- Enhanced Installer with support a new Revit 2027 installation path.
- Added `JetBrains.Annotations` package references to solution templates with private assets.
- Updated `ModularPipelines` to 3.1.6.
- Updated `TUnit` to 1.12.111.
- Updated `WixSharp` to 2.12.1.
- Updated `Sourcy.DotNet` to 1.1.1.
- Updated `Polyfill` to 9.8.0.

# 6.0.2

## SDK

- Added support for Revit 2027.
- Fixed Visual Studio launch profile support.
- Updated `Polyfill` to 9.7.0.

## Solution template

- Updated `Sourcy.DotNet` to 1.0.0.
- Updated assertions in `CreateBundleModule`.

## Revit Test (TUnit) template

- Updated `TUnit` to 1.9.81.
- Added `EnableTUnitPolyfills` property.

# 6.0.1

Minor fixes.

## Solution template

- Improved package conditional directives.
- Updated build commands in documentation.

## MSBuild SDK for Revit Add-ins

- Updated Nuget repository URL.

# 6.0.0

## Global changes

- New `Nice3point.Revit.Sdk` for project configuration and development.
- New templates: `Revit Benchmark` and `Revit Test (TUnit)`.
- Replaced `Nuke` with `ModularPipelines` build system. Nuke is no longer maintained: https://github.com/nuke-build/nuke/discussions/1564#discussioncomment-15001502
- Support for Revit 2026.
- Integrated `GitVersion.Tool` for automatic release versioning based on Git history.
- Integrated automatic changelog generation via GitHub API.

## Solution template

- Migrated to `ModularPipelines`.
- Support for WIX4 and .NET Core installer project.
- Updated GitHub Actions and Azure DevOps pipelines to support new versioning and publishing logic.
- Removed `Nuke` related files and configurations.
- Support for .NET 10.

> [!NOTE]
> The solution .slnx format is broken in Rider and VS and temporary disabled until it will be fixed.
> Use .sln instead and convert to .slnx using or .NET CLI after creating the project.

> [!NOTE]
> The Rider `Create .git repository` option is broken and overrides .gitignore files.
> Uncheck this option and initialize git after creating the project.

## Add-in templates

- Switched to `Nice3point.Revit.Sdk`. Boilerplate code in `.csproj` has been significantly reduced.
- Improved Dependency Injection support.
- C# 14 features support.
- New `LaunchRevit` property for easier debugging.
- New `DeployAddin` property (renamed from `DeployRevitAddin`).

## MSBuild SDK for Revit Add-ins

- New `Nice3point.Revit.Sdk` with pre-configured MSBuild targets and props.

An updated .csproj looks like this:

```msbuild

<Project Sdk="Nice3point.Revit.Sdk/6.0.0">

    <PropertyGroup>
        <DeployAddin>true</DeployAddin>
        <LaunchRevit>true</LaunchRevit>
        <IsRepackable>false</IsRepackable>
        <EnableDynamicLoading>true</EnableDynamicLoading>
        <Configurations>Debug.R22;Debug.R23;Debug.R24;Debug.R25;Debug.R26</Configurations>
        <Configurations>$(Configurations);Release.R22;Release.R23;Release.R24;Release.R25;Release.R26</Configurations>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="Nice3point.Revit.Toolkit" Version="$(RevitVersion).*"/>
        <PackageReference Include="Nice3point.Revit.Extensions" Version="$(RevitVersion).*"/>
        <PackageReference Include="Nice3point.Revit.Api.RevitAPI" Version="$(RevitVersion).*"/>
    </ItemGroup>

</Project>
```

No complex settings, frameworks and versions configuration, SDK takes care of it.

### Migration from v5 to v6

#### Solution migration

1. Delete `.nuke` folder and `build` folder.
2. Delete `installer` folder.
3. Create a temporary project using version 6.0.0 and copy the new `build`, `installer` folders to your solution.
4. Update `global.json` to use .NET 10.
5. Replace your `.github/workflows` or `azure-pipelines.yml` with the new versions from the template.

#### Project migration

1. Update the `Sdk` attribute in your `.csproj` file to `Nice3point.Revit.Sdk/6.0.0`.
2. Remove redundant properties from `.csproj`: `RevitVersion`, `TargetFramework`, `RuntimeIdentifier`, `StartProgram`, `StartArguments`.
3. Remove conditional `PropertyGroup` containing `RevitVersion` and `TargetFramework`.
4. Remove `Nice3point.Revit.Build.Tasks` PackageReference.
5. Rename `DeployRevitAddin` to `DeployAddin`.
6. Rename configurations from `Debug R25` to `Debug.R25` (replace space with dot). Names with spaces are not supported by BenchmarkDotNet and Unit test in JetBrains Rider.

# 5.0.0

## Solution template

- The release publishing pipeline has been completely redesigned. Publishing is now based on tags instead of automatic push to `main` branch. This is aimed at better control to avoid unexpected situations. Also improved pre-release publishing, and releases from any branch, e.g. it is now possible to release an `Alpha` version from the `develop` branch. See [Wiki](https://github.com/Nice3point/RevitTemplates/wiki/Publishing-the-Release) for more details.
- The installer now ignores all `.pdb` files.
- Improved `Readme` file, added all detailed documentation about building and publishing the project. `Readme` file is dynamic and depends on the settings specified when creating Solution.
- Code coverage with documentation.
- Reworked `.yml` files.
- Simplified some code.
- Added support for .NET 9.
- Added support for Revit 2026.
- Removed support for Revit 2020. For support, add it manually by [guide](https://github.com/Nice3point/RevitTemplates/wiki/Managing-API-compatibility).

### Solution migration from v4 to v5

1. Create a completely clean project with the same name based on v5 of the template.
2. Copy the following folders and files to your working project with replacement:
    - `build` folder.
    - `install` folder.
    - `Readme.md` file.
    - `.yml` files.
3. Review the Git Diff carefully:
    - Keep your custom GUIDs and project names.
    - Preserve any user-specific customizations.
    - Roll back any changes to your business logic.
4. Update your solution's dependencies to match v5 requirements.
5. Test the build process to ensure everything compiles correctly.

## Add-in templates

- Enabled `Nullable` by default.
- Added `IsRepackable` property, disabled by default. [Read more](https://github.com/Nice3point/RevitTemplates/wiki/Publishing-the-Release#dependency-conflicts).
- Added `ManifestSettings` section to manifest for enabling dependency isolation, starting with Revit 2026 API.
- Added more WPF converters.
- Fixed typos.
- Updated dependencies.
- Added support for Revit 2026.
- Removed support for Revit 2020.

### Add-in migration from v4 to v5

1. Update your `.csproj` file:
   ```xml
   <!-- Replace this line -->
   <PublishAddinFiles>true</PublishAddinFiles>

   <!-- With these lines -->
   <DeployRevitAddin>true</DeployRevitAddin>
   <EnableDynamicLoading>true</EnableDynamicLoading>
   ```

2. Update your `.addin` file by adding the `ManifestSettings` to enable add-in isolation in the Revit 2026:
   ```xml
   <RevitAddIns>
     <!-- ... existing settings ... -->
      <ManifestSettings>
          <UseRevitContext>False</UseRevitContext>
          <ContextName>RevitAddIn</ContextName>
      </ManifestSettings>
   </RevitAddIns>
   ```

3. Review and update any nullable reference types in your code as they are now enabled by default.

# 4.0.7

- Moved commands from the Module template.
  Please keep the commands in the Primary project that contains External Application to avoid isolation issues.
  Issue: https://github.com/Nice3point/RevitToolkit/issues/7
- Removed WindowsController class to simplify templates
- Updated descriptions

# 4.0.6

- Conditions for generating the Solution readme. The documentation considers which options you have created the solution with
- Replace GetService with GetRequiredService for DI templates
- Rename some default files that use DI
- Updated summary

# 4.0.5

- Updated dependencies
- Removed dependencies conditions with https://github.com/Nice3point/RevitToolkit/releases/tag/2025.0.1 latest changes integration
- Updated samples .csproj

# 4.0.4

- Removed template engine "isEnabled" property. Visual Studio does not support it compared to Jetbrains Rider.
  This property caused an exception to create a Module project without UI.
  Thanks to @SergeyNefyodov for finding this bug

# 4.0.3

- Remove Core folder. It's seldom used, especially in small plugins, so it's removed by default. Thank you for your feedback
- Update templates description
- Disable dotnet clean logo for solution template

# 4.0.2

- Integrate the latest Revit.Build.Tasks features
- Cleanup csproj files

# 4.0.1

- Revit 2025 support
- Inversion of Control support
- Nuke 8.0.0 support
- New icons
- New templates for single dll applications and modular solutions
- New samples https://github.com/Nice3point/RevitTemplates/tree/develop/samples
- Wiki updated https://github.com/Nice3point/RevitTemplates/wiki/Templates
- Wiki updated https://github.com/Nice3point/RevitTemplates/wiki/Multiple-Revit-Versions
- Jetbrains Rider don't respect solution templates for now. Please use CLI or VS22

# 3.2.2

- Fix Github release version validation

# 3.2.1

- Fix Github release version validation

# 3.2.0

- .NET 8 support.
- Redesigned project structure, added `source` folder for storing projects.
- Redesigned Nuke project. More checks, logs, compare url record for GitHub releases. Now you can run on a local machine without having a local git repository.
- Added PackageContent.xml file support for bundles.
- Update bundle structure for Design Automation

# 3.1.1

- Nuke 7.0.0 support
- Build project reworked with using source generators
- The version of the projects is now unified and set for all projects in the Build.Configuration file. Version applies to installer and .dll packages
- Installer project reworked
- Installer project now supports SingleUser and MultiUser builds
- Installer project now supports feature tree for Revit versions

  ![image](https://github.com/Nice3point/RevitTemplates/assets/20504884/d5a3431d-7704-422c-8eba-9c06a00cf0a3)
- New MS Build target. Triggers when the project is cleaned up and deletes the plugin files in the revit addin folder
- Full changelog: https://github.com/Nice3point/RevitTemplates/compare/3.0.1...3.1.1
