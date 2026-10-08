---
name: revit-templates-authoring
description: >
  Add or change a dotnet new template or a template option in the Nice3point.Revit.Templates package.
  USE FOR: declaring template.json parameter and computed symbols, wiring conditional content through source modifiers and inline preprocessor regions, mapping custom operations per file type, exposing an option in both the IDE and the CLI host files.
  DO NOT USE FOR: authoring the Nice3point.Revit.Sdk props, targets, or MSBuild task classes; consuming the shipped templates to scaffold an add-in.
license: MIT
---

# Revit Templates Authoring

Each template in `source/Nice3point.Revit.Templates/<TemplateName>/` is a project that compiles on its own, annotated for the .NET template engine through its `.template.config/` folder.
The template content and its `template.json` symbols define every feature a consumer scaffolds.
No generator runs outside the template engine.

## When to use

- Adding a new template, or a new option to an existing template.
- Changing how an existing option shapes the generated output.
- Fixing a scaffolding combination that produces an invalid project.

## When not to use

- The multi-version build, target-framework selection, version constants, implicit usings, manifest patching, or publishing. The `revit-templates-sdk` skill covers that work.

## Workflow

### Step 1: Declare the parameter symbol in template.json

A parameter symbol declares an option the consumer sets.
Declare it under `symbols` in `.template.config/template.json` with a `datatype`, a `description`, and a `defaultValue` that matches the most common choice.

```json
"addinDiMode": {
  "type": "parameter",
  "displayName": "Dependency Injection",
  "datatype": "choice",
  "description": "Dependency Injection implementation for object lifetime management",
  "choices": [
    { "choice": "disabled",  "displayName": "Disabled",           "description": "The add-in will not use DI" },
    { "choice": "container", "displayName": "Service container",  "description": "Use Microsoft.Extensions.DependencyInjection implementation" },
    { "choice": "hosting",   "displayName": "Hosting",            "description": "Use Microsoft.Extensions.Hosting implementation" }
  ],
  "defaultValue": "disabled"
}
```

A two-state option such as `addinUiWpf` or `addinLogging` uses `"datatype": "bool"` with a `true` or `false` default.

### Step 2: Derive computed symbols from the parameters

A computed symbol names a flag derived from the parameters.
Every condition elsewhere in the template references a single computed symbol.

```json
"diContainer": { "type": "computed", "value": "addinDiMode == \"container\"" },
"diHosting":   { "type": "computed", "value": "addinDiMode == \"hosting\"" },
"useDi":       { "type": "computed", "value": "addinDiMode != \"disabled\"" },
"useUi":       { "type": "computed", "value": "addinUiWpf && addinManifestType != \"dbApplication\"" }
```

### Step 3: Exclude the content with source modifiers and inline regions

Exclude a whole file with a source modifier conditioned on a computed symbol, and a block inside a kept file with an inline preprocessor region.

`template.json` declares the source modifiers under `sources[].modifiers`:

```json
"sources": [
  {
    "modifiers": [
      { "condition": "!useDi",               "exclude": [ "Host.cs" ] },
      { "condition": "!useUi",               "exclude": [ "Models/**", "Views/**", "ViewModels/**" ] },
      { "condition": "isDbApplicationAddin", "exclude": [ "Commands/**" ] }
    ]
  }
]
```

An inline region uses the delimiter of the file's language.
A C# file uses the native `#if`, which both the template engine and the C# compiler process:

```csharp
#if (diHosting && isApplicationAddin)
    public override async Task OnStartupAsync()
#else
    public override void OnStartup()
#endif
```

An MSBuild project file has no preprocessor and wraps the directive in an XML comment.
The template engine removes the whole comment line from the output:

```xml
<!--#if (addinLogging || useDi)-->
    <IsRepackable>true</IsRepackable>
<!--#else-->
    <IsRepackable>false</IsRepackable>
<!--#endif-->
```

### Step 4: Map custom operations for non-standard file types

The template engine defines the delimiters for C# and MSBuild files by default.
Declare the delimiters of any other file type under `SpecialCustomOperations`, keyed by glob.
Without such an entry, the directives reach the generated output verbatim.
The `.addin` manifest uses a bare `#if` with whole-line trimming:

```json
"SpecialCustomOperations": {
  "*.addin": {
    "operations": [
      {
        "type": "conditional",
        "configuration": {
          "if": [ "#if" ], "else": [ "#else" ], "elseif": [ "#elseif" ], "endif": [ "#endif" ],
          "trim": "true", "wholeLine": "true"
        }
      }
    ]
  }
}
```

The Solution template maps more file types: `README.md` uses `---#if`, a token that never collides with markdown; `*.slnx` uses `#if`; and `*.json` uses `#if` with `"trim": "false"` and `"wholeLine": "false"` for inline conditionals.

### Step 5: Expose the option in both host files

An option reaches the consumer only when both host files list it.
Add it to `ide.host.json` for the New Project dialog, with the symbol listed under `symbolInfo` and `"persistenceScope": "shared"`:

```json
{ "id": "addinDiMode", "persistenceScope": "shared" }
```

Add it to `dotnetcli.host.json` to map the symbol to a command-line flag, and extend `usageExamples`:

```json
"symbolInfo": {
  "addinDiMode": { "longName": "di", "shortName": "" }
},
"usageExamples": [
  "dotnet new revit-addin",
  "dotnet new revit-addin --addin command --wpf --di hosting --logger"
]
```

An "open in editor" post action has the condition `HostIdentifier != "dotnetcli"` and does not run on the command line.
Its `args.files` value is an index into `primaryOutputs`.

### Step 6: Mirror the change in the sample

Each option set has a runnable skeleton under `samples/` that mirrors the output of a template.
A sample references `Nice3point.Revit.Sdk` by published version, and a template references it by name.
A template change that alters the generated output updates the matching sample in the same commit.

### Step 7: Extend the wiki, the CHANGELOG, and the option matrix

Document the option in the consumer wiki page under `wiki/` and record it in `CHANGELOG.md` under the `## Templates` heading.
Add the new combination to `GenerateMatrix()` in `build/Modules/TestTemplatesModule.cs`.
The template test scaffolds and builds every combination that `GenerateMatrix()` returns.

### Step 8: Verify by scaffolding and compiling the matrix

Run the template test from the `build` directory.
The test packs both packages to a local feed, installs the template package, generates a project for every combination of the option matrix, and builds each generated project against its first release configuration.

```shell
dotnet run -- test
```

## Validation

- [ ] Every parameter symbol has a `datatype`, `description`, and `defaultValue`, and every condition references a computed symbol.
- [ ] Each option appears in both `ide.host.json` and `dotnetcli.host.json`, with a new `usageExamples` entry where it changes the usage.
- [ ] A file other than C# or MSBuild that contains directives has a matching `SpecialCustomOperations` entry.
- [ ] The matching `samples/` project reflects the change.
- [ ] `GenerateMatrix()` covers the new combination, and `dotnet run -- test` scaffolds and compiles it.
- [ ] The wiki page and `CHANGELOG.md` are updated in the same commit.

## Common Pitfalls

| Pitfall                                                    | Correct approach                                                                                             |
|------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------|
| Adding a symbol to `template.json` only                    | List it in `ide.host.json` and `dotnetcli.host.json` as well.                                                |
| Bare `#if` in a `.addin`, `.slnx`, `.json`, or `README.md` | Register the delimiters under `SpecialCustomOperations`.                                                     |
| Writing scaffolding logic in a generator or the SDK        | Express the option as template content and `template.json` symbols that the template engine processes.       |
| Changing template content but not the sample               | Update the matching `samples/` project in the same commit.                                                   |
| Adding an option without extending `GenerateMatrix()`      | Add the combination to `GenerateMatrix()`; the template test builds no other combination.                    |
| Losing the namespace form of `sourceName`                  | Keep the `RootNamespace` override. `safe_namespace` maps `Nice3point.Revit.AddIn.1` to the `._1` namespace.  |
