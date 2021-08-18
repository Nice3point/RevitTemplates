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
  * .Net Core (Builder)
* Revit Versions
  * 2019
  * 2020
  * 2021
  * 2022
* Package contents
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

Not everything is automated now. Here are some things to do after creating a new project:

* Switch the project manager from **Solution** to **File System** and move the **.run** folder from the .csproj folder to the .sln folder.
* Move the **.nuke** folder from the installation directory to the solution folder. Rename the solution name in the **parameters.json** file and the project names in the **Build.cs** file.
* By default, **VendorId** and **AssemblyCompany** are set to **"CompanyName"**. To replace this field with your own value, press the key combination **Ctrl + Shift + R**.
