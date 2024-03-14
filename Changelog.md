# 4.0.0-preview.2.0

- Downgrade dependencies for Revit 2025 compability
- Rename some files
- Bug fixes

# 4.0.0-preview.1.0

- Revit 2025 support
- Inversion of Control support
- New templates for single dll applications and modular solutions
- New samples https://github.com/Nice3point/RevitTemplates/tree/develop/samples
- Wiki updated https://github.com/Nice3point/RevitTemplates/wiki/Templates

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