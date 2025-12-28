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

![image](https://github.com/user-attachments/assets/7d2ea9c6-33e0-4b3f-b4eb-a195bb79ae21)

# Revit AddIn Application

Suitable for modular add-ins.
Option for developers who want to create a distributed scalable application.

This template creates a main empty application containing only the entry point file and `.addin` manifest.

Optionally, you can enable dependency injection and support, where this application becomes a dependency provider for other modules.

> [!NOTE]  
> Project based on this template should be used to combine all modules and connect them to the Revit ribbon, and it should not contain business logic if you plan to create a modular application.

![image](https://github.com/user-attachments/assets/8780b0e0-22ce-40b7-b970-fa3acd3f723e)

# Revit AddIn Module

Suitable for modular add-ins.
Option for developers who want to create a distributed scalable application.

This template creates an empty module with or without user interface, containing files and dependencies necessary to implement the business logic.

After creating a project based on this template, you have to add a `reference` to this project from [Revit AddIn Application](#revit-addin-application)

> [!TIP]  
> Creating a project without UI based on this template will create a completely empty project, great for writing util libraries for your add-in.

![image](https://github.com/user-attachments/assets/abedaa19-5e66-41f2-bedb-f43c5dac1d06)

# Revit AddIn Solution

Solution template. 
Suitable for enterprise development and developers who need a ready-made project structure with all core files, build system and installer.

This template contains:

- `ModularPipelines` build system
- Installer project, that generates an **.msi** package
- Auxiliary files such as `.gitignore`, `Changelog.md` which are usually created in each solution. `Readme.md` contains documentation and instructions for building the project.
- CI\CD setup
- JetBrains Rider `Run configurations`

> [!IMPORTANT]  
> When creating a solution, be sure to check the box **Put solution and project in the same directory**.

> [!TIP]  
> The solution template should be used before the project templates (if you need it).
> And you have to create all plugins in the produced solution, in the `source` folder.

### ModularPipelines

ModularPipelines is used to build a project for various configurations. In this case, for all specified Revit versions. 
It also allows you to automate other secondary processes, create an installer, generate a changelog, etc.

More details about ModularPipelines [here](https://github.com/thomhurst/ModularPipelines).

> [!NOTE]  
> You don't need to use the build system for the add-ins directly, it's only needs for publishing Releases.

### Installer

WixSharp was chosen as the installer, it is based on a console application, this helps to make the creation of the installer automated and connect it to the build system.

More details about WixSharp [here](https://github.com/oleg-shilo/wixsharp).

![image](https://github.com/user-attachments/assets/10835ebb-a08d-434a-ba9e-a7abbd3b61f8)
