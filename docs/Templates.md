All templates are fully supported by .Net, and you can use them in your favourite IDE, such as **Visual Studio**, **JetBrains Rider** or CLI.

These templates supports Revit 2021-2026 versions out of the box and can be extended without any limitations.

More information about dotnet templates: https://github.com/dotnet/templating/wiki

<!-- TOC -->
# Templates:

* [Revit AddIn](#revit-addin) - template for creating single-project add-in.
* [Revit AddIn Application](#revit-addin-application) - template for creating an empty application for multi-project add-in.
* [Revit AddIn Module](#revit-addin-module) - template for creating an empty module for multi-project add-in.
* [Revit AddIn Solution](#revit-addin-solution) - template for creating a solution structure for Revit add-ins.
<!-- TOC -->

If you are not sure what options to choose when creating a project, keep everything by default, templates uses optimal and frequently used settings. 
Or explore the [samples](https://github.com/Nice3point/RevitTemplates/tree/develop/samples) before you start.

# Revit AddIn

Suitable for single project add-ins. 
Perfect choice for small projects.

Just create a project, and it will already be ready to run in Revit.

- The template already contains startup configurations, and you can run debugging for any Revit version without any setup.
- Support for multiple Revit versions and different .Net versions is available by default.
- The template provides the ability to create add-ins with or without a user interface, and more experienced developers can enable dependency injection and logging.
- Choose whatever features you want, it's all optional.

![image](https://github.com/Nice3point/RevitTemplates/assets/20504884/5a5a98c6-725a-48a5-ac5e-3aec889b47cb)

# Revit AddIn Application

Suitable for modular add-ins.
Option for developers who want to create a distributed scalable application.

This template creates a main empty application containing only the entry point file and `.addin` manifest.

Optionally, you can enable dependency injection and support, where this application becomes a dependency provider for other modules.

> [!NOTE]  
> Project based on this template should be used to combine all modules and connect them to the Revit ribbon, and it should not contain business logic if you plan to create a modular application.

![image](https://github.com/Nice3point/RevitTemplates/assets/20504884/f44ccf0c-0a5e-4d47-874b-94ca0d5b0a85)

# Revit AddIn Module

Suitable for modular add-ins.
Option for developers who want to create a distributed scalable application.

This template creates an empty module with or without user interface, containing files and dependencies necessary to implement the business logic.

After creating a project based on this template, you have to add a `reference` to this project from [Revit AddIn Application](#revit-addin-application)

> [!TIP]  
> Creating a project without UI based on this template will create a completely empty project, great for writing util libraries for your add-in.

![image](https://github.com/Nice3point/RevitTemplates/assets/20504884/28f8e900-34e2-431a-8054-c29f1e901f78)

# Revit AddIn Solution

Solution template. 
Suitable for enterprise development and developers who need a ready-made project structure with all core files, build system and installer.

This template contains:

- `ModularPipelines` build system
- Installer project, that generates an .msi package
- Auxiliary files such as `.gitignore`, `Changelog.md` which are usually created in each solution. `Readme.md` contains documentation and instructions for building the project.
- CI\CD setup
- JetBrains Rider `Run configurations`

> [!IMPORTANT]  
> When creating a solution, be sure to check the box **Put solution and project in the same directory**.

> [!TIP]  
> The solution template should be used before the project templates (if you need it).
And you have to create all plugins in produced solution, in the `source` folder.

### ModularPipelines

ModularPipelines is used to build a project for various configurations. In this case, for all specified Revit versions. It also allows you to automate other secondary processes, create an
installer, generate a changelog, etc.

More details about ModularPipelines [here](https://github.com/thomhurst/ModularPipelines).

> [!NOTE]  
> You don't need to use the build system to debug the project, it is only necessary for the publishing release.

### Installer

WixSharp was chosen as the installer, it is based on a console application, this helps to make the creation of the installer automated and connect it to the build system.

More details about WixSharp [here](https://github.com/oleg-shilo/wixsharp).

![image](https://github.com/Nice3point/RevitTemplates/assets/20504884/51e8ca10-68fd-48a5-8bef-63bb7b49d142)