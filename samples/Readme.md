Add-in examples created with these templates are suitable for beginners and advanced users.
You can create your own add-ins with different settings or completely empty.
Templates will take care of project configuration and multi Revit version compability.

Just create a project, and it will already be ready to run in Revit.

Templates description: https://github.com/Nice3point/RevitTemplates/wiki/Templates

## Basic

- **[Single-project Application](https://github.com/Nice3point/RevitTemplates/tree/develop/samples/SingleProjectApplication)** — 
  a simple add-in without a user interface, perfect for beginners.
  It contains only one command and a button on the Revit ribbon to run it.

| Template    | Creation options              | Value                |
|-------------|-------------------------------|----------------------|
| Revit AddIn | AddIn type<br/>User interface | Application<br/>None |

- **[Single-project WPF Application (Modal)](https://github.com/Nice3point/RevitTemplates/tree/develop/samples/SingleProjectWpfModalApplication)** —
  an add-in with a user interface that blocks Revit UI.
  Suitable for developers who are starting to learn WPF or already working with it.
  It contains all necessary dependencies and implements the MVVM pattern.

| Templates   | Creation options              | Value                 |
|-------------|-------------------------------|-----------------------|
| Revit AddIn | AddIn type<br/>User interface | Application<br/>Modal |

- **[Single-project WPF Application (Modeless)](https://github.com/Nice3point/RevitTemplates/tree/develop/samples/SingleProjectWpfModelessApplication)** —
  an add-in with a user interface that does not block Revit UI.
  Suitable for developers who want to implement a modeless window.
  Contains all necessary dependencies, examples of using **IExternalEventHandler** (calling Revit API from another thread) and asynchronous calls.
  It implements the MVVM pattern.

| Templates   | Creation options              | Value                    |
|-------------|-------------------------------|--------------------------|
| Revit AddIn | AddIn type<br/>User interface | Application<br/>Modeless |

## Advanced

- **[Single-project Application with Dependency Injection](https://github.com/Nice3point/RevitTemplates/tree/develop/samples/SingleProjectDIApplication)** —
  option for developers who need a simple implementation of Dependency injection using service containers with `Microsoft.Extensions.DependencyInjection` package.

| Template    | Creation options                      | Value                                       |
|-------------|---------------------------------------|---------------------------------------------|
| Revit AddIn | AddIn type<br/>User interface<br/>IoC | Application<br/>Modal<br/>Service container |

- **[Single-project Application with Hosting](https://github.com/Nice3point/RevitTemplates/tree/develop/samples/SingleProjectHostingApplication)** —
  option for developers who need an advanced IoC option with logging, metrics, options and configurations using `Microsoft.Extensions.Hosting` package.

| Template    | Creation options                      | Value                             |
|-------------|---------------------------------------|-----------------------------------|
| Revit AddIn | AddIn type<br/>User interface<br/>IoC | Application<br/>Modal<br/>Hosting |

- **[Multi-project Application](https://github.com/Nice3point/RevitTemplates/tree/develop/samples/MultiProjectApplication)** —
  modular application, where each add-in is placed in a separate project, and orchestrated by main add-in that connects all modules to the Revit ribbon.
  Including sample with **ExtensibleStorage** for a database.

| Template                | Creation options | Value       |
|-------------------------|------------------|-------------|
| Revit AddIn Application | AddIn type       | Application |
| Revit AddIn Module      | User interface   | None        |
| Revit AddIn Module      | User interface   | Modal       |

## Enterprise

- **[Multi-project Solution with Hosting](https://github.com/Nice3point/RevitTemplates/tree/develop/samples/MultiProjectSolution)** —
  an example suitable for enterprise development with build system implementation and automation of all application production processes.

  Included:
    - Solution structure
    - Installer producing
    - Bundle producing (publishing to Autodesk Store or Forge)
    - CI/CD using GitHub
    - MVVM pattern implementation with modal and modeless windows
    - Dependency Injection implementation using Hosting
    - Logging
    - **IOptions\<T\>** usages for serialization
    - **IExternalEvents** usages for calling Revit API from async code

| Template                | Creation options                                  | Value                                  |
|-------------------------|---------------------------------------------------|----------------------------------------|
| Revit Solution          | Bundle support<br/>Installer support<br/>Pipeline | Enabled<br/>Enabled<br/>GitHub actions |
| Revit AddIn Application | Serilog support<br/>AddIn type<br/>IoC            | Enabled<br/>Application<br/>Hosting    |
| Revit AddIn Module      | Inject dependencies<br/>User interface            | Enabled<br/>Modal                      |
| Revit AddIn Module      | Inject dependencies<br/>User interface            | Enabled<br/>Modeless                   |