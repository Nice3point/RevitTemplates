<p align="center">
    <picture>
        <source media="(prefers-color-scheme: dark)" width="750" srcset="https://github.com/Nice3point/RevitTemplates/assets/20504884/cb0992f1-927f-4937-a87b-0e9657318c05">
        <img alt="RevitTemplates" width="750" src="https://github.com/Nice3point/RevitTemplates/assets/20504884/ddeb2dd2-e3e9-46f8-a643-4176a09c8560">
    </picture>
</p>

## Create your Add-In for Revit on the .NET platform

[![Nuget](https://img.shields.io/nuget/vpre/Nice3point.Revit.Templates?style=for-the-badge)](https://www.nuget.org/packages/Nice3point.Revit.Templates)
[![Downloads](https://img.shields.io/nuget/dt/Nice3point.Revit.Templates?style=for-the-badge)](https://www.nuget.org/packages/Nice3point.Revit.Templates)
[![Last Commit](https://img.shields.io/github/last-commit/Nice3point/RevitTemplates/develop?style=for-the-badge)](https://github.com/Nice3point/RevitTemplates/commits/develop)

This repository contains a collection of project templates for creating Revit Add-Ins.

## Template Features

- Multi-target Revit API version support
- WPF support with MVVM architecture
- Dependency Injection support
- Integrated Serilog support for structured logging
- Custom MSBuild SDK for simplified development
- Assembly isolation support for .NET Core builds
- Modern build system for building, testing and packaging
- Automatic versioning for releases
- Ready-to-use installer and Autodesk Store bundle projects
- Pre-configured CI/CD pipelines for GitHub and Azure DevOps

## Installation

1. Install the latest [.NET SDK](https://dotnet.microsoft.com/download).
2. Run the following command to install the templates:
   ```shell
   dotnet new install Nice3point.Revit.Templates
   ```

## Usage

- IDE: select one of the Revit templates from the New Project dialog in JetBrains Rider or Visual Studio.
- CLI: run `dotnet new revit-addin` or other available [templates](https://github.com/Nice3point/RevitTemplates/blob/main/docs/Templates.md).

For more information, read [Step-by-step Guide](https://github.com/Nice3point/RevitTemplates/blob/main/docs/Step%E2%80%90by%E2%80%90step-Guide.md) and check [Wiki](https://github.com/Nice3point/RevitTemplates/wiki).