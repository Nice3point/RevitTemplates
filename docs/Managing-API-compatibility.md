## Table of content

<!-- TOC -->
* [Conditional compilation for a specific Revit version](#conditional-compilation-for-a-specific-revit-version)
  * [Examples](#examples)
* [Managing Supported Revit Versions](#managing-supported-revit-versions)
  * [Solution configurations](#solution-configurations)
  * [Project configurations](#project-configurations)
* [API references](#api-references)
<!-- TOC -->

## Conditional compilation for a specific Revit version

To write code compatible with different Revit versions, use the directives **#if**, **#elif**, **#else**, **#endif**.

```c#
#if REVIT2026
    //Your code here
#endif
```

To target a specific Revit version, set the solution configuration in your IDE interface to match that version.
E.g., select the `Debug R26` configuration for the Revit 2026 API.

The project has available constants such as `REVIT2026`, `REVIT2026_OR_GREATER`, `REVIT2026_OR_GREATER`. 
Create conditions, experiment to achieve the desired result.

> [!NOTE]
> For generating directives, a third-party package is used.
> You can find more detailed documentation about it here: [Revit.Build.Tasks](https://github.com/Nice3point/Revit.Build.Tasks)

### Examples

To support the latest APIs in legacy Revit versions:

```c#
#if REVIT2021_OR_GREATER
    UnitUtils.ConvertFromInternalUnits(69, UnitTypeId.Millimeters);
#else
    UnitUtils.ConvertFromInternalUnits(69, DisplayUnitType.DUT_MILLIMETERS);
#endif
```

`#if REVIT2021_OR_GREATER` сompiles a block of code for Revit versions 21, 22, 23 and greater.

To support removed APIs in newer versions of Revit, you can invert the constant:

```c#
#if !REVIT2023_OR_GREATER
    var builtinCategory = (BuiltInCategory) category.Id.IntegerValue;
#endif
```

`#if !REVIT2023_OR_GREATER` сompiles a block of code for Revit versions 22, 21, 20 and lower.

## Managing Supported Revit Versions

To extend or reduce the range of supported Revit API versions, you need to update the solution and project configurations.

### Solution configurations

Solution configurations determine which projects are built and how they are configured. 

To support multiple Revit versions:
- Open the `.sln` file.
- Add or remove configurations for each Revit version.

Example:

```text
GlobalSection(SolutionConfigurationPlatforms) = preSolution
    Debug R24|Any CPU = Debug R24|Any CPU
    Debug R25|Any CPU = Debug R25|Any CPU
    Debug R26|Any CPU = Debug R26|Any CPU
    Release R24|Any CPU = Release R24|Any CPU
    Release R25|Any CPU = Release R25|Any CPU
    Release R26|Any CPU = Release R26|Any CPU
EndGlobalSection
```

For example `Debug R26` is the Debug configuration for Revit 2026 version.

> [!TIP]  
> If you are just ending maintenance for some version, removing the Solution configurations without modifying the Project configurations is enough.

### Project configurations

Project configurations define build conditions for specific versions.

To add or remove support:
- Open `.csproj` file
- Add or remove configurations for Debug and Release builds.

Example:

```xml
<PropertyGroup>
    <Configurations>Debug R24;Debug R25;Debug R26</Configurations>
    <Configurations>$(Configurations);Release R24;Release R25;Release R26</Configurations>
</PropertyGroup>
```

> [!IMPORTANT]  
> Edit the `.csproj` file only manually, IDEs often break configurations.

Then simply map the solution configuration to the project configuration:

![image](https://github.com/user-attachments/assets/9f357ded-d38c-4f0a-a21f-152de75f4abc)

Solution and project configuration names may differ, this example uses the same naming style to avoid confusion.

Then specify the framework and Revit version for each configuration, update the `.csproj` file with the following:

```xml
<PropertyGroup Condition="$(Configuration.Contains('R26'))">
    <RevitVersion>2026</RevitVersion>
    <TargetFramework>net8.0-windows</TargetFramework>
</PropertyGroup>
```

## API references

To support CI/CD pipelines and build a project for Revit versions not installed on your computer, use Nuget packages.

> [!NOTE]   
> Revit API dependencies are available in the [Revit.API](https://github.com/Nice3point/RevitApi) repository.

The Nuget package version must include wildcards `Version="$(RevitVersion).*"` to automatically include adding a specific package version, depending on the selected solution
configuration.

```xml
<ItemGroup>
    <PackageReference Include="Nice3point.Revit.RevitAPI" Version="$(RevitVersion).*"/>
    <PackageReference Include="Nice3point.Revit.RevitAPIUI" Version="$(RevitVersion).*"/>
</ItemGroup>
```