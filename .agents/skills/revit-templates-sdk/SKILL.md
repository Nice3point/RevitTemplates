---
name: revit-templates-sdk
description: >
  Author the Nice3point.Revit.Sdk MSBuild SDK that scaffolded Revit add-ins build against, covering its props and targets contract and the C# MSBuild task classes.
  USE FOR: parsing the configuration into a Revit version and selecting the target framework, emitting REVIT#### and REVIT####_OR_GREATER constants, adding conditional implicit usings, patching the add-in manifest, publishing and repacking output, and loading the task assembly under every MSBuild host.
  DO NOT USE FOR: declaring template options or conditional content in the template package; consuming the SDK to build an add-in.
license: MIT
---

# Revit Templates SDK

`Nice3point.Revit.Sdk` is the custom MSBuild SDK a scaffolded add-in references through `Sdk="Nice3point.Revit.Sdk"`.
The SDK derives the build settings from the active configuration, and the project file of the add-in stays almost empty.
The project `source/Nice3point.Revit.Sdk` contains the props and targets contract in `Sdk/` and the MSBuild task classes at the project root.
The SDK owns the multi-version build, and template content duplicates none of its logic.

## When to use

- Changing configuration parsing, target-framework selection, or the version constants.
- Changing the implicit usings, manifest patching, publishing, or repacking targets.
- Adding or changing an MSBuild task class in the SDK assembly.

## When not to use

- Declaring a scaffolding option or conditional content. The `revit-templates-authoring` skill covers that work.

## Workflow

### Step 1: Keep the props and targets import order

`Sdk/Sdk.props` imports the Microsoft SDK props first, then `Nice3point.Revit.Common.props`.
`Sdk/Sdk.targets` imports `Nice3point.Revit.Common.targets`, then the Microsoft SDK targets, then each feature target file.
A feature target hooks the standard build through `AfterTargets` / `BeforeTargets`, never by replacing a built-in target.

### Step 2: Derive the Revit version and target framework in Common.props

`Nice3point.Revit.Common.props` parses the trailing number of the configuration name, normalizes a two-digit form to the full year, and falls back to `-1` when the name contains no number.
The `ValidateRevitVersion` target of `Nice3point.Revit.Common.targets` runs when the version is `-1` and fails the build with an error.

```xml
<_RevitConfigurationVersion>$([System.Text.RegularExpressions.Regex]::Match($(Configuration), '(\d+)(?!.*\d)').Value)</_RevitConfigurationVersion>
<RevitVersion Condition="'$(_RevitConfigurationVersion.Length)' == '4'">$(_RevitConfigurationVersion)</RevitVersion>
<RevitVersion Condition="'$(_RevitConfigurationVersion.Length)' == '2'">20$(_RevitConfigurationVersion)</RevitVersion>
<RevitVersion Condition="'$(RevitVersion)' == ''">-1</RevitVersion>
```

`Nice3point.Revit.Common.props` then selects the `TargetFramework` from `RevitVersion` thresholds.
Each configuration of one project builds against the framework of its Revit version.

### Step 3: Write an MSBuild task class that returns, never throws

A task class at the SDK project root derives from `Microsoft.Build.Utilities.Task` and has the `[PublicAPI]` attribute.
Mark a mandatory input `[Required]` and a returned value `[Output]`.
`Execute` wraps its body in a `try`/`catch`, logs a failure and returns `false`, and returns `true` as a no-op when nothing applies.
No exception leaves `Execute`.

```csharp
[PublicAPI]
public class GenerateCompatibleDefineConstants : Task
{
    [Required] public required string Configuration { get; set; }
    public string[] Configurations { get; set; } = [];
    public string? RevitVersion { get; set; }
    [Output] public string[]? DefineConstants { get; private set; }

    public override bool Execute()
    {
        try
        {
            int currentVersion;
            if (string.IsNullOrEmpty(RevitVersion))
            {
                if (!TryGetRevitVersion(Configuration, out currentVersion)) return true;
            }
            else
            {
                if (!int.TryParse(RevitVersion, out currentVersion)) return true;
            }

            //Business logic
            ...

            return true;
        }
        catch (Exception exception)
        {
            Log.LogErrorFromException(exception, false);
            return false;
        }
    }
}
```

This task emits `REVIT####` for the active version and `REVIT####_OR_GREATER` for the active version and every earlier configuration.
Template content gates a version-specific Revit API with `#if REVIT2024_OR_GREATER`.

### Step 4: Register the task and hook it from a feature target

Register each task with `UsingTask` in `Sdk.props`, with the task name prefixed by `$(MSBuildThisFileName)` and `AssemblyFile` set to the task assembly that Step 5 selects.

```xml
<UsingTask TaskName="$(MSBuildThisFileName).GenerateCompatibleDefineConstants" AssemblyFile="$(BuildTasksAssembly)"/>
```

Invoke the task from a feature target ordered against the standard build, and assign its `[Output]` to the property or item that consumes it.

```xml
<Target Name="GenerateRevitCompatibleDefineConstants"
        AfterTargets="AddImplicitDefineConstants"
        Condition="'$(DisableImplicitRevitDefines)' != 'true'">
    <GenerateCompatibleDefineConstants RevitVersion="$(RevitVersion)" Configuration="$(Configuration)" Configurations="$(Configurations)">
        <Output TaskParameter="DefineConstants" ItemName="_ImplicitRevitDefineConstant"/>
    </GenerateCompatibleDefineConstants>
    <PropertyGroup>
        <DefineConstants Condition="'@(_ImplicitRevitDefineConstant)' != ''">$(DefineConstants);@(_ImplicitRevitDefineConstant)</DefineConstants>
    </PropertyGroup>
</Target>
```

### Step 5: Keep the task assembly loadable under every build host

`Nice3point.Revit.Sdk.csproj` multi-targets `net48;netstandard2.0;net10.0`.
The task assembly loads under MSBuild on .NET Framework, older .NET Core hosts, and the latest SDK.
`Sdk.props` selects the matching folder before it registers the tasks.

```xml
<BuildTasksTFM Condition="'$(MSBuildRuntimeType)' == 'Core' AND $([MSBuild]::VersionGreaterThanOrEquals($(NETCoreSdkVersion), '10.0'))">net10.0</BuildTasksTFM>
<BuildTasksTFM Condition="'$(MSBuildRuntimeType)' == 'Core' AND '$(BuildTasksTFM)' == ''">netstandard2.0</BuildTasksTFM>
<BuildTasksTFM Condition="'$(BuildTasksTFM)' == ''">net48</BuildTasksTFM>
```

### Step 6: Verify against the build and keep the README in sync

The csproj packs `wiki/MsBuild-Sdk.md` as the package README.
A change to a public property or a feature updates that wiki page in the same commit.
Run the template test from the `build` directory.
The test scaffolds a project for every combination of the option matrix and builds each against the SDK.

```shell
dotnet run -- test
```

## Validation

- [ ] `Sdk.props` and `Sdk.targets` keep the import order, and every feature target hooks the standard build through `AfterTargets` / `BeforeTargets`.
- [ ] `RevitVersion` and `TargetFramework` derive from the configuration, and an unresolved version fails the build through `ValidateRevitVersion`.
- [ ] Every public task class has `[PublicAPI]`, marks inputs `[Required]` and results `[Output]`, and returns `false` on failure without throwing.
- [ ] Each task is registered with `UsingTask` and invoked from a feature target ordered against the standard build.
- [ ] The csproj multi-targets every build host, and `Sdk.props` selects the matching task folder.
- [ ] `wiki/MsBuild-Sdk.md` reflects any public property or feature change, and `dotnet run -- test` compiles the scaffolded matrix.

## Common Pitfalls

| Pitfall                                                      | Correct approach                                                                              |
|--------------------------------------------------------------|-----------------------------------------------------------------------------------------------|
| Throwing from a task on bad input                            | Return `true` as a no-op when nothing applies, and `false` after `Log.LogErrorFromException`. |
| Replacing a built-in target                                  | Hook the standard build with `AfterTargets` / `BeforeTargets`.                                |
| Dropping a task TFM from the csproj                          | Keep `net48;netstandard2.0;net10.0` in `TargetFrameworks`.                                    |
| Duplicating build logic into template content                | Author configuration parsing, TFM selection, constants, and publishing once in the SDK.       |
| Renaming a public SDK property or task without a deprecation | Rename only through a deprecation path, and keep `[PublicAPI]` on the task.                   |
| Editing the packed README directly                           | Update `wiki/MsBuild-Sdk.md`, the source of the package README.                               |
