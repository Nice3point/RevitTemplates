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

## First steps

Solution:

* Before building the project, you need to edit the solution file.
  Make sure that the Installer project is built only in the Installer configuration, remove lines from other configurations.
  Below are examples of solution and projects configurations:

  | Section             | Example configurations               |
  | ------------------- | ------------------------------------ |
  | Solution            | ![](https://i.imgur.com/LnnjYYu.png) |
  | Installer project   | ![](https://i.imgur.com/uW9Wxjp.png) ![](https://i.imgur.com/OhVDh6m.png) |
  | Main project        | ![](https://i.imgur.com/XpxVFcB.png) ![](https://i.imgur.com/53auQ0K.png) ![](https://i.imgur.com/TuVKQrZ.png)|

* Remove all unnecessary configurations from the solution, the **build** project should not build. The solution file
  might look like this:

  | File   | Example                              |
  | ------ | ------------------------------------ |
  | .sln   | ![](https://i.imgur.com/3VQQtwQ.png) |
