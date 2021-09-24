<h3 align="center"><img src="https://i.imgur.com/v34P0ro.png" width="500px"></h3>

# Create your Add-In for Revit on the .Net platform now.

<p align="center">
  <a href="https://www.nuget.org/packages/Nice3point.Revit.Templates"><img src="https://img.shields.io/nuget/v/Nice3point.Revit.Templates?style=for-the-badge"></a>
  <a href="https://github.com/Nice3point/RevitTemplate/commits/main"><img src="https://img.shields.io/github/last-commit/Nice3point/RevitTemplate?style=for-the-badge"></a>
</p>
This repository contains a project templates for creating Revit Add-In.

## Template Features

* Platforms
    * .Net Framework
    * .Net Core
* Revit Versions
    * Dynamically expandable
    * Latest tested version - 2022
* Package content
    * Solution template
    * Add-In template

## Installation

1. Install the latest [.Net SDK](https://dotnet.microsoft.com/download).
1. Run `dotnet new -i Nice3point.Revit.Templates` to install the project templates.

## Usage

* For creating project run `dotnet new revit-sln -n ProjectName`, where ProjectName is the name of your project
* For testing, switch the solution configuration to Debug, and run the Run/Debug configuration with the appropriate
  revit version. The .dll and .addin files will be automatically copied to the Revit add-ons folder.
* To build the Release version, run **nuke** command or use ready-made .yml files for building in the cloud. More details about Nuke
  [here](https://github.com/nuke-build/nuke).
* For more help read [Wiki](https://github.com/Nice3point/RevitTemplates/wiki)
