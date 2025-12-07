# Revit MsBuild Sdk

MSBuild Sdk for developing and publishing the plugin for multiple Revit versions.

## Table of contents

<!-- TOC -->
* [Preprocessor symbols](#preprocessor-symbols)
* [Publishing](#publishing)
  * [Local deployment](#local-deployment)
  * [Publishing for distribution](#publishing-for-distribution)
  * [Publish extra content](#publish-extra-content)
* [Assembly repacking](#assembly-repacking)
* [Manifest patching](#manifest-patching)
* [Implicit global usings](#implicit-global-usings)
* [Configuration](#configuration)
<!-- TOC -->

### Preprocessor symbols

Preprocessor symbols (`#define` constants) are used in conditional compilation to enable or exclude code based on the target Revit version.
This ensures compatibility across multiple Revit versions without code duplication.

The `OR_GREATER` symbols are cumulative and provide a cleaner way to handle version-specific API changes.
Each symbol indicates compatibility with the specified version and all newer versions.

| Current configuration | Project configurations               | Generated define constants                                                  |
|-----------------------|:-------------------------------------|-----------------------------------------------------------------------------|
| Debug R20             | Debug R20, Release R21, Release 2022 | REVIT2020, REVIT2020_OR_GREATER                                             |
| Release R21           | Debug R20, Release R21, Release 2022 | REVIT2021, REVIT2020_OR_GREATER, REVIT2021_OR_GREATER                       |
| Release 2022          | Debug R20, Release R21, Release 2022 | REVIT2022, REVIT2020_OR_GREATER, REVIT2021_OR_GREATER, REVIT2022_OR_GREATER |

Usage:

```C#
#if REVIT2021_OR_GREATER
    UnitUtils.ConvertFromInternalUnits(69, UnitTypeId.Millimeters);
#else
    UnitUtils.ConvertFromInternalUnits(69, DisplayUnitType.DUT_MILLIMETERS);
#endif
```

To support removed APIs in newer versions of Revit, you can invert the constant:

```C#
#if !REVIT2023_OR_GREATER
    var builtinCategory = (BuiltInCategory) category.Id.IntegerValue;
#endif
```

Constants are generated from the names of project configurations. If your project configurations do not contain metadata about the version, you can specify it explicitly:

```xml
<PropertyGroup>
    <RevitVersion>2025</RevitVersion>
</PropertyGroup>
```

Preprocessor symbols generating is enabled by default, to disable implicit defines, set the `DisableImplicitRevitDefines` property:

```xml
<PropertyGroup>
    <DisableImplicitRevitDefines>true</DisableImplicitRevitDefines>
</PropertyGroup>
```

### Publishing

Depending on your workflow, you can either deploy the files locally for immediate testing and debugging or publish them into a folder for further distribution.

#### Local deployment

To copy Revit add-in files to the `%AppData%\Autodesk\Revit\Addins` folder after building a project, you can enable the `DeployAddin` property.

Copying files helps attach the debugger to the add-in when Revit starts. This makes it easier to test the application or can be used for local development.

```xml
<PropertyGroup>
    <DeployAddin>true</DeployAddin>
</PropertyGroup>
```

_Default: Disabled_

Should only be enabled in projects containing the Revit manifest file (`.addin`).

`Clean solution` or `Clean project` commands will delete the deployed files.

#### Publishing for distribution

If your goal is to generate an installer or a bundle, enable the `PublishAddin` property.
This configuration publishes the compiled files into the `bin\publish` folder.

```xml
<PropertyGroup>
    <PublishAddin>true</PublishAddin>
</PropertyGroup>
```

_Default: Disabled_

#### Publish extra content

By default, all project files and dependencies required for the plugin to run, including the `.addin` manifest, are copied.
If you need to include additional files, such as configuration or family files, include them in the `Content` item.

```xml
<ItemGroup>
    <Content Include="Resources\Families\Window.rfa" CopyToPublishDirectory="Always"/>
    <Content Include="Resources\Music\Click.wav" CopyToPublishDirectory="PreserveNewest"/>
    <Content Include="Resources\Images\**" CopyToPublishDirectory="PreserveNewest"/>
</ItemGroup>
```

To enable copying Content files, set `CopyToPublishDirectory="Always"` or `CopyToPublishDirectory="PreserveNewest"`

The `PublishDirectory` property specifies which subfolder of the plugin the file should be copied to.
If it is not specified, the files will be copied to the root folder.

```xml
<ItemGroup>
    <Content Include="Resources\Families\Window.rfa" PublishDirectory="Families" CopyToPublishDirectory="PreserveNewest"/>
    <Content Include="Resources\Music\Click.wav" PublishDirectory="Music\Effects" CopyToPublishDirectory="PreserveNewest"/>
    <Content Include="Resources\Images\**" PublishDirectory="Images" CopyToPublishDirectory="PreserveNewest"/>
    <Content Include="Readme.md" CopyToPublishDirectory="PreserveNewest"/>
</ItemGroup>
```

Result:

```text
📂bin\publish; %AppData%\Autodesk\Revit\Addins\2025
 ┣📜RevitAddIn.addin
 ┗📂RevitAddIn
   ┣📂Families
   ┃ ┗📜Family.rfa
   ┣📂Images
   ┃ ┣📜Image.png
   ┃ ┣📜Image2.png
   ┃ ┗📜Image3.jpg
   ┣📂Music
   ┃ ┗📂Effects
   ┃   ┗📜Click.wav
   ┣📜CommunityToolkit.Mvvm.dll
   ┣📜RevitAddIn.dll
   ┗📜Readme.md
```

### Assembly repacking

Assembly repacking is used to merge multiple assemblies into a single Dll, primarily to avoid dependency conflicts between different add-ins.

If you need to repack assemblies into a single Dll, enable the `IsRepackable` property.
[ILRepack](https://www.nuget.org/packages/ILRepack/) package is required.

```xml
<PropertyGroup>
    <IsRepackable>true</IsRepackable>
</PropertyGroup>
```

_Default: false_

To exclude certain assemblies from repacking if they cause unexpected behavior, specify them using the `RepackBinariesExcludes` property:

```xml
<PropertyGroup>
    <RepackBinariesExcludes>$(AssemblyName).UI.dll;System*.dll</RepackBinariesExcludes>
</PropertyGroup>
```

Wildcards are supported.
All binaries are repacked into the **bin** directory after the build.

For .NET Core applications, it is recommended to disable this feature and use **Dependency Isolation**, which is available starting from Revit 2026.

### Manifest patching

By default, enabled target is used to modify the Revit `.addin` manifest to ensure backward compatibility between different Revit versions.

For example, if the manifest includes nodes or properties, which is only supported in newest Revit version, it will be removed for older versions:

**Original `.addin` manifest:**

```xml
<RevitAddIns>
    <AddIn Type="Application">
        <Name>RevitAddin</Name>
        <Assembly>RevitAddin\RevitAddin.dll</Assembly>
    </AddIn>
    <ManifestSettings>
        <UseRevitContext>False</UseRevitContext>
    </ManifestSettings>
</RevitAddIns>
```

**Patched `.addin` manifest for Revit 2025 and older:**

```xml
<RevitAddIns>
    <AddIn Type="Application">
        <Name>RevitAddin</Name>
        <Assembly>RevitAddin\RevitAddin.dll</Assembly>
    </AddIn>
</RevitAddIns>
```

Target is triggered automatically if the `PublishAddin` property is enabled.

### Implicit global usings

By default, included a target for generating implicit global Usings depending on the project references. Helps to reduce the frequent use of `using` in a project.

| Global Using                                | Enabled by reference            |
|---------------------------------------------|---------------------------------|
| using Autodesk.Revit.DB;                    | RevitAPI.dll                    |
| using JetBrains.Annotations;                | JetBrains.Annotations.dll       |
| using Nice3point.Revit.Extensions;          | Nice3point.Revit.Extensions.dll |
| using Nice3point.Revit.Toolkit;             | Nice3point.Revit.Toolkit.dll    |
| using CommunityToolkit.Mvvm.Input;          | CommunityToolkit.Mvvm.dll       |
| using CommunityToolkit.Mvvm.ComponentModel; | CommunityToolkit.Mvvm.dll       |

To disable implicit usings, set the `ImplicitRevitUsings` property:

```xml
<PropertyGroup>
    <ImplicitRevitUsings>false</ImplicitRevitUsings>
    <!--OR-->
    <ImplicitUsings>false</ImplicitUsings>
</PropertyGroup>
```

### Configuration

This package overrides some properties for the optimal add-in development:

| Property                          | Default value | Description                                                                                 |
|-----------------------------------|---------------|---------------------------------------------------------------------------------------------|
| AppendTargetFrameworkToOutputPath | false         | Prevents the TFM from being appended to the output path. Required to publish an application |

These properties are automatically applied to the `.csproj` file, but can be overriden:

```xml
<PropertyGroup>
    <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
</PropertyGroup>
```