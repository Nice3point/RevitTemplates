# 5.0.0-preview.2.250317

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

- Create a completely clean project with the same name based on v5 of the template.
- Copy the `build` folder to your working project with replacement.
- Copy the `install` folder to your working project with replacement.
- Copy the `Readme.md` file into your working project with replacement.
- Copy the `.yml` files into your working project with replacement.
- Check the Git Diff, and perform a rollback for user-customized lines, changed Guids or names.

## Addin templates

- Enabled `Nullable` by default.
- Added `IsRepackable` property, disabled by default. [Read more](https://github.com/Nice3point/RevitTemplates/wiki/Publishing-the-Release#dependency-conflicts).
- Added `ManifestSettings` section to manifest for enabling dependency isolation, starting with Revit 2026 API.
- Added more WPF converters.
- Fixed typos.
- Updated dependencies.
- Added support for Revit 2026.
- Removed support for Revit 2020.

### Addin migration from v4 to v5

- Replace the `PublishAddinFiles` property with `DeployRevitAddin` in the `.csproj` file.
- Add the `EnableDynamicLoading` property and set it to `true` in the `.csproj` file.
- Update `.addin` file and add the `ManifestSettings` section according to the Autodesk manual.

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