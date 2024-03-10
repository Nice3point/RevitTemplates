Plugin examples created with templates are suitable for beginners and advanced users.
You can create your own add-ins with different settings, there and completely empty, templates will take care of the solution customisation and setup configurations.

Just create a project, and it will already be ready to run in Revit.

## Basic

- **[SingleApplication](https://github.com/Nice3point/RevitTemplates/tree/develop/samples/SingleApplication)** - a simple add-in without a user interface, perfect for beginners.
  It contains only one command and a button on the Revit ribbon to run it.

| Template    | Option                        | Value                |
|-------------|-------------------------------|----------------------|
| Revit AddIn | AddIn type<br/>User interface | Application<br/>None |

- **[SingleUiApplication](https://github.com/Nice3point/RevitTemplates/tree/develop/samples/SingleUiApplication)** - An add-in with a user interface that blocks Revit.
  Suitable for those who are starting to learn WPF or already working with it.
  It contains all necessary dependencies and implements MVVM pattern.

| Templates   | Option                        | Value                 |
|-------------|-------------------------------|-----------------------|
| Revit AddIn | AddIn type<br/>User interface | Application<br/>Modal |

- **[SingleUiModelessApplication](https://github.com/Nice3point/RevitTemplates/tree/develop/samples/SingleUiModelessApplication)** - An add-in with a user interface that does not block Revit.
  Suitable for those who want to implement a modeless window.
  Contains all necessary dependencies, examples of using IExternalEventHandler and asynchronous calls.
  It implements MVVM pattern.

| Templates   | Option                        | Value                    |
|-------------|-------------------------------|--------------------------|
| Revit AddIn | AddIn type<br/>User interface | Application<br/>Modeless |

## Advanced

- **[SingleDependencyInjectionApplication](https://github.com/Nice3point/RevitTemplates/tree/develop/samples/SingleDependencyInjectionApplication)** - option for those who need a simple implementation of Dependency injection using containers.
  With examples of using View and ViewModel injections. Uses the Microsoft.Extensions.DependencyInjection library.

| Template    | Option                                | Value                                       |
|-------------|---------------------------------------|---------------------------------------------|
| Revit AddIn | AddIn type<br/>User interface<br/>IoC | Application<br/>Modal<br/>Service container |

- **[SingleHostingApplication](https://github.com/Nice3point/RevitTemplates/tree/develop/samples/SingleHostingApplication)** - option for those who need an advanced IoC option using Hosting, with logging, metrics and configurations.
  With examples of using View and ViewModel injections. Uses the Microsoft.Extensions.Hosting library.

| Template    | Option                                | Value                             |
|-------------|---------------------------------------|-----------------------------------|
| Revit AddIn | AddIn type<br/>User interface<br/>IoC | Application<br/>Modal<br/>Hosting |

- **[ModularApplication](https://github.com/Nice3point/RevitTemplates/tree/develop/samples/ModularApplication)** - modular add-in, where each plug-in is placed in a separate project, and orchestrated in the main application that connects all modules to the Revit
  ribbon. Contains ExtensibleStorage implementation

| Template                | Option         | Value       |
|-------------------------|----------------|-------------|
| Revit AddIn Application | AddIn type     | Application |
| Revit AddIn Module      | User interface | None        |
| Revit AddIn Module      | User interface | Modal       |

## Enterprise

- **[AllInOneSolution](https://github.com/Nice3point/RevitTemplates/tree/develop/samples/AllInOneSolution)** - an example suitable for enterprise development with implemented build system, automation of all processes, installer and bundle (publishing in Autodesk
  Store or Forge) creation, CI/CD support, with logging and DI.

| Template                | Option                                            | Value                                  |
|-------------------------|---------------------------------------------------|----------------------------------------|
| Revit Solution          | Bundle support<br/>Installer support<br/>Pipeline | Enabled<br/>Enabled<br/>GitHub actions |
| Revit AddIn Application | Serilog support<br/>AddIn type<br/>IoC            | Enabled<br/>Application<br/>Hosting    |
| Revit AddIn Module      | Inject view<br/>User interface                    | Enabled<br/>Modal                      |
| Revit AddIn Module      | Inject view<br/>User interface                    | Enabled<br/>Modeless                   |
