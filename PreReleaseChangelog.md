- .Net SDK 6 support
- Nuke 6 support
- C# 10 support
- Implicit usings
- File-scoped namespaces
- The revit-sln template is now displayed in the IDE
- Added group Solution Items with frequently used non-project files
- Added SubTransaction
- Added extensions for adding images to a button
- Added copying .pdb files to Revit folder for non-Rider debugger
- Optimized .csproj file for revit-addin template. Added generate AssemblyInfo (not a file)
- Replaced Nuget packages with floating versions
- YML files switched to windows-2022 image
- Hidden bin folder from explorer
- Installer is optional now
- Fixed changelog parser
- Other minor changes and improvements

Wiki updated:

- https://github.com/Nice3point/RevitTemplates/wiki/Multiple-Revit-Versions
- https://github.com/Nice3point/RevitTemplates/wiki/Third-party-files
- https://github.com/Nice3point/RevitTemplates/wiki/Modular-solution
- https://github.com/Nice3point/RevitTemplates/wiki/Multiple-Installer-Versions
- https://github.com/Nice3point/RevitTemplates/wiki/Autodesk-Store-bundle

Visual Studio 2022 supports custom parameters, we create projects without using a terminal. Just remember when creating a solution to check "Put solution and project in same
directory".

If you use Rider and want to add support for custom parameters, put your thumbs up:

- https://youtrack.jetbrains.com/issue/RIDER-16759

Also vote for bug fixes:

- https://youtrack.jetbrains.com/issue/RIDER-71014
- https://youtrack.jetbrains.com/issue/RIDER-66638