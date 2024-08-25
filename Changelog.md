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