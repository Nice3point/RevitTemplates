<h3 align="center"><img src="https://i.imgur.com/v34P0ro.png" width="500px"></h3>

# Create your Add-In for Revit on the .Net platform now.

<p align="center">
  <a href="https://www.nuget.org/packages/Nice3point.Revit.Templates"><img src="https://img.shields.io/nuget/v/Nice3point.Revit.Templates?style=for-the-badge"></a>
  <a href="https://www.nuget.org/packages/Nice3point.Revit.Templates"><img src="https://img.shields.io/nuget/dt/Nice3point.Revit.Templates?style=for-the-badge"></a>
  <a href="https://github.com/Nice3point/RevitTemplate/commits/main"><img src="https://img.shields.io/github/last-commit/Nice3point/RevitTemplate?style=for-the-badge"></a>
</p>
This repository contains a project templates for creating Revit Add-In.

## Template Features

- Build projects for all versions of Revit.
- The templates have all the necessary files for a quick start.
- Maximum automation with the **Nuke** build system.
- Automatic **Installer** creation.
- Automatically publishing a Release to **GitHub**.
- Creating a Bundle to publish an Application to the **Autodesk Store**.
- Support for **Azure DevOps** pipelines and **GitHub Actions**.
- Creating a plugin of **Application** and **Command** types.

## Installation

1. Install the latest [.Net SDK](https://dotnet.microsoft.com/download).
1. Run `dotnet new -i Nice3point.Revit.Templates` to install the project templates.

## Usage

- To create a project, use the **New Project/Solution** button in your IDE or run `dotnet new` command in a terminal.
- To create a package run `nuke`. More details about Nuke [here](https://github.com/nuke-build/nuke).
- For more help read [Wiki](https://github.com/Nice3point/RevitTemplates/wiki).