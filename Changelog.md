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