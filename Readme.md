<h3 align="center"><img src="https://i.imgur.com/wkZAO9B.png" width="500px"></h3>

Create your add-in for Revit on the .Net platform now.
==================================================
<p align="center">
  <a href="https://github.com/Nice3point/RevitTemplate/issues"><img src="https://img.shields.io/github/issues/Nice3point/RevitTemplate"></a>
  <a href="https://github.com/Nice3point/RevitTemplate/commits/main"><img src="https://img.shields.io/github/last-commit/Nice3point/RevitTemplate"></a>
</p>
This repository contains a project template for creating Revit Add-ins.

Template Features
------------
* Supported IDEs
  * JetBrains Rider
* Platforms
  * .Net Framework
  * .Net Core
* Revit Versions
  * 2019
  * 2020
  * 2021
  * 2022
* Package content
  * AddIn template
  * Installer template
  * Nuke builder template
  
Installation
------------

**Rider**:
1. Download the project
1. Click the New Solution button
1. Go to the More Templates section
1. Select template folder
1. Click Reload Button

Usage
------------

Switch the project manager from **Solution** to **File System**

AddIn: 
* Move the **.run** folder to the solution folder. Rider Run/Debug configurations here.

Installer:
* Rename the project name in the **Installer.cs** file. It will be displayed during installation.
* Rename the output name in the **Installer.cs** file. This will be the name of the installer file.

Builder: 
* Rename the solution name in the **parameters.json** file.
* Rename the project names in the **Build.Properties.cs** file.
* Move the **.nuke** folder to the solution folder.
* If you are using CI, the project has a customized **azure-pipelines.yml** file and a **.github** folder. Move them to the root of the solution if needed.
* By default, creating a bundle for uploading to AutodeskStore is disabled. To enable, remove the **Skip** line in the **parameters.json** file.

Solution:
* For the installer to work, a configuration must be created in the solution, begins with field value of the **InstallerConfiguration** field in the **Build.Properties.cs** file, by default it is the "Installer". If you want to make unique installers, the name of the main project configuration must have a suffix like the installer project configuration, for example, for the **Release R22 Store** configuration, there must be a **Release Store** installer configuration, the Revit R22 version is ignored.
 Below are examples of solution and projects configurations:
  
  | Section | Example configurations |
  | ----------- | ----------- |
  | Solution      | ![](https://i.imgur.com/LnnjYYu.png) |
  | Installer project   | ![](https://i.imgur.com/uW9Wxjp.png) ![](https://i.imgur.com/OhVDh6m.png) |
  | Main project   | ![](https://i.imgur.com/XpxVFcB.png) ![](https://i.imgur.com/53auQ0K.png) ![](https://i.imgur.com/TuVKQrZ.png)|