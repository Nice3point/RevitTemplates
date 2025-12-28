In this guide, we will walk you through each step of working with templates, from installation to deployment.

You will learn how to create a project, debug an application, and build an installer. All steps are demonstrated using JetBrains Rider IDE. 
If you’re using Visual Studio, don’t worry — the steps are nearly identical.

## Table of content:

<!-- TOC -->
  * [Installation](#installation)
  * [Create a New Project](#create-a-new-project)
    * [Project Wizard Options](#project-wizard-options)
    * [Configuring the Solution](#configuring-the-solution)
    * [Debugging](#debugging)
  * [Creating Additional Modules](#creating-additional-modules)
    * [Create a New Project](#create-a-new-project-1)
    * [Adding a New Module](#adding-a-new-module)
    * [Linking the Module](#linking-the-module)
  * [Creating a Solution](#creating-a-solution)
    * [Create a New Solution:](#create-a-new-solution)
    * [Solution Setup](#solution-setup)
    * [Customizing the Build System:](#customizing-the-build-system)
    * [Building the Installer](#building-the-installer)
    * [Running the Build](#running-the-build)
  * [Publishing a Release](#publishing-a-release)
<!-- TOC -->

## Installation

To install templates, simply run the following command in your terminal:
`dotnet new install Nice3point.Revit.Templates`

![image](https://github.com/user-attachments/assets/1a8edc40-26bc-43a4-a8b8-d3bb5ec3c87d)

The templates will be installed, and you're ready to use.

## Create a New Project

To create a new project, click **New Solution** in Rider or **Create a new Project** if you are using Visual Studio:

![image](https://github.com/user-attachments/assets/02c10b90-5819-4635-8a51-2c8f50a90340)

### Project Wizard Options:

1. Select the desired [template](Templates.md), such as Revit Addin for a single-project add-in.
2. Name your project and choose the directory.
3. Choose additional settings, such as whether to include WPF. If unsure, stick with the default settings.

![image](https://github.com/user-attachments/assets/0dbfe226-ad35-4887-9c06-b584a4332dfa)

Your project is now ready to launch.

### Configuring the Solution:

1. Choose the appropriate solution configuration to compile for a specific Revit version.
2. **Debug.R25** compiles the solution for Revit 2025. The last two digits indicate the Revit [version](Managing-API-compatibility.md).
3. **Run configurations** determine which application starts when debugging. Select your main application.
4. Start debugging by clicking the appropriate button. Revit will launch automatically.

![image](https://github.com/user-attachments/assets/9fe821f9-baf1-4bf0-ae13-27df1d812ba1)

### Debugging:

Debugging is pretty simple, without any additional setup. To debug:

1. Set a breakpoint where you need to inspect the code.
2. Click the add-in button in the Revit ribbon.
3. The program will halt at the breakpoint, allowing you to inspect local variables.

![image](https://github.com/user-attachments/assets/a6f3e0f4-61df-4abe-b1e9-9d8bcf9605ae)

## Creating Additional Modules

For larger projects, a single-project solution may not be sufficient. 
Splitting your plugin into multiple modules is a best practice when managing unrelated processes.

To add another project to your solution, we will [use](Templates.md) the **Revit AddIn Module** and **Revit AddIn Application** templates.

### Create a New Project:

1. Select the [template](Templates.md), in this case, **Revit AddIn Application**, which has fewer settings and files than `Revit AddIn`.
2. Name the project and choose its directory.
3. Choose additional settings, such as adding DependencyInjection support. Use the defaults if you're unsure.

![image](https://github.com/user-attachments/assets/1207b4cf-dbf0-4d50-b9ed-e8de0c1fcc7d)

### Adding a New Module:

You will get a created application, however without modules, let's fix that.
Right-click on your solution and select **Add → New Project**:

![image](https://github.com/user-attachments/assets/c19045bc-4afa-43ef-aad5-beb27ab4d2cb)

Configure the new module:
1. Choose the **Revit AddIn Module** [template](Templates.md).
2. Name the module and select its directory.
3. Configure additional settings, like whether to include WPF.

![image](https://github.com/user-attachments/assets/4774aebc-d993-4326-aa6c-1df94c4e15fc)

### Linking the Module:

Add a reference to this module in your main project:

![image](https://github.com/user-attachments/assets/6a610f5c-fc7f-4993-9953-c6613bd20e5b)

You can now add an **ExternalCommand** to execute code from this module, which you can trigger with a button click from the Revit ribbon:

![image](https://github.com/user-attachments/assets/e9c97157-5773-4b22-ac43-953edeebd524)

## Creating a Solution

For enterprise-level development, you might need a more structured solution, including a build system and an installer.

### Create a New Solution:
1. Select the solution [template](Templates.md), in this case **Revit Addin Solution**.
2. Name the solution and choose the directory.
3. Configure additional settings, such as whether to include an installer. Defaults are fine if you’re unsure.

![image](https://github.com/user-attachments/assets/fced0ced-2985-4c9f-9ba1-0868f945ccc5)

### Solution Setup:

Your solution will include a pre-built folder structure, the **ModularPipelines** build system, and a **Readme** file with user instructions.

Follow the steps from previous sections to add projects to this solution:

![image](https://github.com/user-attachments/assets/60e9f1de-ae57-45fe-971d-3e6bc5a3a2a6)

### Customizing the Build System:

Let's customize the build system:
1. In the **build** project, you will find frequently used configuration files, right now we need **appsettings.json**.
2. Update the **BundleOptions** section if you plan to publish your application to the Autodesk Store.
3. The build system automatically finds all projects in the **source** folder, so no additional mapping is required.

![image](https://github.com/user-attachments/assets/80bc14c9-d10a-4d4a-8360-70f3d5aff242)

### Building the Installer:

The build system is based on a console application and does not require the installation of additional tools. You just need to have the .NET SDK installed on your computer.

![image](https://github.com/user-attachments/assets/12e25bf2-dfe4-4d84-9108-cc9a7e33322d)

### Running the Build:

Before running the build system, you must initialize a Git repository and make the first commit. This is required for **GitVersion.Tool** to automatically determine the version and search for files.

1. Open a terminal and navigate to the solution root.
2. Initialize Git and make the first commit:
   ```shell
   git init
   git add .
   git commit -m "Initial commit"
   ```
3. Open the **Readme.md** file in your solution for instructions on building the installer, including publishing to GitHub.
4. Run the following command:
   ```shell
   dotnet run --project build/Build.csproj
   ```
   Alternatively, you can use the **Run configurations** in Rider to start the build process without using the terminal.

![image](https://github.com/user-attachments/assets/dbce742c-f363-4955-8190-d36d92b19a7f)

After a successful build, navigate to the solution folder:

![image](https://github.com/user-attachments/assets/8f7ced27-8b44-4c8f-b080-9f930d3f113d)

You will find the installer in the `output` directory:

![image](https://github.com/user-attachments/assets/08aa5bee-a76b-4dc2-ad7d-56e8c4cdb97b)

## Publishing a Release

There are several ways to create a Release, the easiest way — GitHub Actions:

1. Navigate to the **Actions** section on the repository page.
2. Select **Publish Release** workflow.
3. Click **Run workflow** button.
4. (Optional) Specify the release version and click **Run**.

   ![image](https://github.com/user-attachments/assets/088388c1-6055-4d21-8d22-70f047d8f104)

For the more advanced, or developers who want more control over releases, go to the [publishing](Publishing-the-Release.md) page.
The build system automatically determines the Release version using Git tags. If no tag is specified, it will automatically be generated by GitVersion.Tool.

To include Release notes, you also need to update the changelog file.
Updating the changelog is optional. If you provide a changelog, the build system will use it for the release notes. Otherwise, GitHub will automatically generate them.

To update the changelog manually:

1. Navigate to the solution root.
2. Open the file **Changelog.md**.
3. Add a section for your version. The version separator is the `#` symbol.
4. Specify the release number e.g. `# 1.0.0` or `# Release v1.0.0`, the format does not matter, the main thing is that it contains the version.
5. In the lines below version, write a changes for this version. Style to your taste.

---

The build system is as flexible as possible, you can customize it to suit to your project usage.

Practice creating your plugins, and remember that templates are just a starting point. Your journey has just begun.