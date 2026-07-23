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
A feature a consumer scaffolds lives in the template content and its `template.json` symbols, never in a generator that runs outside the engine.

## When to use

- Adding a new template, or a new option to an existing template.
- Changing how an existing option shapes the generated output.
- Fixing a scaffolding combination that produces an invalid project.

## When not to use

- The multi-version build, target-framework selection, version constants, implicit usings, manifest patching, or publishing. That is SDK work; see `revit-templates-sdk`.

## Workflow

### Step 1: Declare the parameter symbol in template.json

A parameter symbol is the question a consumer answers.
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

Use `"datatype": "bool"` with a `true`/`false` default for a two-state option such as `addinUiWpf` or `addinLogging`.

### Step 2: Add computed symbols to read conditions clearly

A computed symbol derives a named flag from the parameters; every condition elsewhere is then a single readable token.

```json
"diContainer": { "type": "computed", "value": "addinDiMode == \"container\"" },
"diHosting":   { "type": "computed", "value": "addinDiMode == \"hosting\"" },
"useDi":       { "type": "computed", "value": "addinDiMode != \"disabled\"" },
"useUi":       { "type": "computed", "value": "addinUiWpf && addinManifestType != \"dbApplication\"" }
```

### Step 3: Exclude the content with source modifiers and inline regions

Drop whole files with a source modifier keyed on a computed symbol, and switch blocks inside a kept file with an inline preprocessor region.

Source modifiers live under `sources[].modifiers` in `template.json`:

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

Inline regions use the delimiter for the file's language.
A C# file uses native `#if`, which both the engine and the compiler understand:

```csharp
#if (diHosting && isApplicationAddin)
    public override async Task OnStartupAsync()
#else
    public override void OnStartup()
#endif
```

An MSBuild project file has no preprocessor; it wraps the directive in an XML comment, and the engine strips the whole comment line:

```xml
<!--#if (addinLogging || useDi)-->
    <IsRepackable>true</IsRepackable>
<!--#else-->
    <IsRepackable>false</IsRepackable>
<!--#endif-->
```

### Step 4: Map custom operations for non-standard file types

The engine knows the delimiters for C# and MSBuild files by default.
For any other file type, declare the delimiters under `SpecialCustomOperations` keyed by glob, or the directives ship verbatim into the output.
The `.addin` manifest uses bare `#if` with whole-line trimming:

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

The solution template maps more types: `README.md` uses `---#if`, a token that never collides with markdown; `*.slnx` uses `#if`; and `*.json` uses `#if` with `"trim": "false"` and `"wholeLine": "false"` for inline conditionals.

### Step 5: Expose the option in both host files

An option is invisible unless it appears in both entry points.
Add it to `ide.host.json` for the New Project dialog, listing the symbol under `symbolInfo` with `"persistenceScope": "shared"`:

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

Gate a "open in editor" post action on `HostIdentifier != "dotnetcli"`; it stays silent on the command line. Index `args.files` into `primaryOutputs`.

### Step 6: Mirror the change in the sample

Each option set has a runnable skeleton under `samples/` that mirrors a template's output, differing only in that it references `Nice3point.Revit.Sdk` by published version, not by name.
A template change that alters the generated output updates the matching sample in the same commit.

### Step 7: Extend the wiki, the CHANGELOG, and the option matrix

Document the option in the consumer wiki page under `wiki/` and record it in `CHANGELOG.md` under the `## Templates` heading.
Add the new combination to `GenerateMatrix()` in `build/Modules/TestTemplatesModule.cs`; the build then scaffolds and compiles the new path.

### Step 8: Verify by scaffolding and compiling the matrix

Run the template test from the `build` directory.
It packs both packages to a local feed, installs the template package, generates a project for every option-matrix combination, and builds each generated project against its first release configuration.

```shell
dotnet run -- test
```

## Validation

- [ ] Every parameter symbol has a `datatype`, `description`, and `defaultValue`, and every condition reads through a computed symbol.
- [ ] Each option appears in both `ide.host.json` and `dotnetcli.host.json`, with a fresh `usageExamples` entry where it changes usage.
- [ ] A non-C#, non-MSBuild file that carries directives has a matching `SpecialCustomOperations` entry.
- [ ] The mirroring `samples/` project reflects the change.
- [ ] `GenerateMatrix()` covers the new combination, and `dotnet run -- test` scaffolds and compiles it.
- [ ] The wiki page and `CHANGELOG.md` are updated in the same commit.

## Common Pitfalls

| Pitfall                                                    | Correct approach                                                                                                |
|------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------|
| Adding a symbol to `template.json` only                    | Also list it in `ide.host.json` and `dotnetcli.host.json`, or it never surfaces to the consumer.                |
| Bare `#if` in a `.addin`, `.slnx`, `.json`, or `README.md` | Register the delimiters under `SpecialCustomOperations`, or the directives leak into the output.                |
| Writing scaffolding logic in a generator or the SDK        | Express the option as template content and `template.json` symbols processed by the engine.                     |
| Changing template content but not the sample               | Update the mirroring `samples/` project in the same commit so the skeleton stays faithful.                      |
| Adding an option without extending `GenerateMatrix()`      | Add the combination; the build then compiles it. An uncompiled combination is unverified.                       |
| Losing the namespace form of `sourceName`                  | `Nice3point.Revit.AddIn.1` maps to the `._1` namespace via `safe_namespace`; keep the `RootNamespace` override. |
