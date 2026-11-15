All templates are fully supported by .NET, and you can use them in your favourite IDE, such as **Visual Studio**, **JetBrains Rider** or CLI.

The templates support Revit 2023-2027 out of the box and can be extended without any limitations.

More information about dotnet templates: https://github.com/dotnet/templating/wiki

<!-- TOC -->
# Templates:

* [Revit AddIn](#revit-addin) - template for creating single-project add-in.
* [Revit AddIn Application](#revit-addin-application) - template for creating an empty application for multi-project add-in.
* [Revit AddIn Module](#revit-addin-module) - template for creating an empty module for multi-project add-in.
* [Revit Service Defaults](#revit-service-defaults) - template for creating a common configuration project for multi-project add-in.
* [Revit AddIn Solution](#revit-addin-solution) - template for creating a solution structure for Revit add-ins.
* [Revit Benchmark](#revit-benchmark) - template for creating performance benchmarks for Revit API.
* [Revit Test](#revit-test) - template for creating unit tests for Revit API.
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

Optionally, you can enable dependency injection, where this application becomes a dependency provider for other modules.
For Logging, diagnostics and the rest of the shared configuration add [Revit Service Defaults](#revit-service-defaults).

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

# Revit Service Defaults

Suitable for modular add-ins.
Option for developers who want one service configuration for the application and all its modules.

The template follows the idea of the [.NET Aspire service defaults](https://learn.microsoft.com/dotnet/aspire/fundamentals/service-defaults): one project holds every service registration the application and its modules share, and a single `AddServiceDefaults` method applies it.
The template provides logging and diagnostics out of the box.
Serialization, HTTP clients, options, and any other shared registration are added to the same project.
The method is declared in the namespace of the host builder, and you call it without an extra `using` directive.

After creating a project based on this template, add a `reference` to this project from [Revit AddIn Application](#revit-addin-application) and call `AddServiceDefaults` in the host:

```c#
builder.AddServiceDefaults();
```

A module references the project too when it consumes a shared type, such as a serializer context.

> [!IMPORTANT]
> Choose the same `Dependency Injection` option the application was created with, it selects the type the defaults are applied to.

# Revit AddIn Solution

Solution template.
Suitable for enterprise development and developers who need a ready-made project structure with all core files, build system and installer.

This template contains:

- `ModularPipelines` build system
- Installer project, that generates an **.msi** package
- Auxiliary files such as `.gitignore`, `CHANGELOG.md` which are usually created in each solution. `README.md` contains documentation and instructions for building the project.
- Code style in `.editorconfig` and the LF line-ending policy in `.gitattributes`
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

The build project runs on ModularPipelines 4 and requires the .NET 11 SDK.
The add-in projects keep the target frameworks of their Revit versions.

The following table lists the folders of the build project:

| Folder     | Contents                                                                             |
|------------|--------------------------------------------------------------------------------------|
| `Pipeline` | The pipeline steps and the settings they read from `appsettings.json`.               |
| `Wix`      | The commands of the WiX Toolset. The folder exists in a solution with the installer. |

The `Pipeline` folder groups the steps by what they do:

| Folder        | Contents                                                                    |
|---------------|-----------------------------------------------------------------------------|
| `Versioning`  | The version of the release and the Revit configurations of the solution.    |
| `Compilation` | The clean and compile steps.                                                |
| `Testing`     | The test step. The folder exists in a solution with tests.                  |
| `Packaging`   | The bundle and installer steps, with the `Bundle` and `Installer` settings. |
| `Publishing`  | The release notes and the GitHub release, with the `Publish` settings.      |

A settings class is in the folder of the steps that read it, and `BuildOptions`, which every folder reads, is in `Pipeline` itself.
The build checks the required settings before the first step runs.
A missing value stops the build with an error that names the setting:

```text
OptionsValidationException: UpgradeCode: The UpgradeCode field is required.
```

ModularPipelines has packages for `dotnet`, Git, and GitHub, and a step calls them through `context.Tools`.
A tool without a package gets a folder of its own, named after the tool.
The folder contains the commands of the tool, their options, and the registration.

The following step accepts the WiX license and adds the UI extension through the commands of the `Wix` folder:

```c#
await context.Tools.Wix.EulaAcceptAsync(new WixEulaAcceptOptions("wix7")
{
    Tool = wixExe.Path
}, cancellationToken: cancellationToken);

await context.Tools.Wix.ExtensionAddAsync(new WixExtensionAddOptions($"WixToolset.UI.wixext/{wixVersion}")
{
    Tool = wixExe.Path,
    Global = true
}, cancellationToken: cancellationToken);
```

In the preceding code:

- `context.Tools.Wix` exists because the `RegisterWixContext` method of the folder has the `[ModularPipelinesIntegration]` attribute.
- Every options record names its command in a `[CliSubCommand]` attribute, and its properties are the arguments of that command.
- `Tool` is the path to the executable. Without it, the command runs `wix` from the `PATH` environment variable.

To call another tool, copy the `Wix` folder, rename its types after the tool, and declare one options record per command.

ModularPipelines saves a report of every run to the `.modularpipelines` folder at the root of the repository.
The `.gitignore` of the solution excludes the folder.

### Installer

WixSharp was chosen as the installer, it is based on a console application, this helps to make the creation of the installer automated and connect it to the build system.

The build writes a manifest with the published add-in for every Revit version, and the `installer` project builds a per-user and a per-machine **.msi** package from it.

The following table lists the folders of the installer project:

| Folder      | Contents                                                                                      |
|-------------|-----------------------------------------------------------------------------------------------|
| `Manifest`  | The manifest classes and their JSON metadata. The build project compiles the same files.      |
| `Layouts`   | The package of an installation scope, its features for every Revit version, and its cabinets. |
| `Resources` | The icon and the images of the installer.                                                     |

To add a value to the installer, such as a publisher name, add a property to `InstallerManifest`, set it in the installer step of the build, and read it in `PackageLayout`.

Set the upgrade code of the product in the `Installer` section of `build/appsettings.json`.
Keep the same code for every release: a release with the same code upgrades the installed product, and a release with a new code is installed side by side with it.

More details about WixSharp [here](https://github.com/oleg-shilo/wixsharp).

![image](https://github.com/user-attachments/assets/10835ebb-a08d-434a-ba9e-a7abbd3b61f8)

# Revit Benchmark

Suitable for performance testing and measuring the execution time of your code directly in the Revit environment.

This template is based on the [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) library, which is the industry standard for benchmarking .NET code.

The template is pre-configured to run benchmarks in the Revit thread and reflect real-world API usage.
It supports multiple Revit versions, and you can compare the performance of different versions of Revit API in one benchmark.

The project runs through `BenchmarkSwitcher`, and command-line arguments select the benchmarks to run:

```shell
dotnet run -c Release.R27 -- --filter '*'
```

# Revit Test

Suitable for unit testing and writing automated tests for your Revit add-ins.

This template is based on the [TUnit](https://github.com/thomhurst/TUnit) library, a modern, fast and flexible testing framework for .NET.

Tests are configured to run inside the Revit process, providing full access to the Revit API. The template supports multiple Revit and .Net versions by default.
Minimal TUnit configuration, and you can write and set up your own tests as you wish.
