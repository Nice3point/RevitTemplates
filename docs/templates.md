# Templates & SDK

This guide covers how to author the templates and the SDK that scaffolded projects build with. The consumer-facing guide to using the templates lives in the wiki. See [Documentation](./documentation.md).

## Template Engine Model

Each template is a real project that compiles on its own, annotated for the .NET template engine. The `.template.config/template.json` file declares the template identity, its short name, and the symbols that drive scaffolding. The template engine resolves the symbols against the consumer's answers and produces the final project.

* **Symbols** declare the options a consumer chooses. A parameter symbol is a question with a datatype and a default. A computed symbol derives a flag from other symbols so conditions read clearly.
* **Conditional content** tailors the output. Source modifiers exclude files for a given condition, and inline preprocessor regions include or drop blocks within a file. The custom operations map the preprocessor syntax onto each file type so a manifest, a project file, and a readme each use the right delimiters.
* **`sourceName`** is the token the engine replaces with the consumer's chosen name across file names and content. Keep it consistent across a template's files.
* **Host files** adapt the options to each entry point. The IDE host file controls option order and persistence in the New Project dialog. The CLI host file maps each symbol to a command-line flag and lists usage examples.
* **Post actions** open the primary output in the editor after scaffolding, gated so they stay silent on the command line.

## Add or Change a Template Option

1. Declare the parameter symbol in `template.json` with a datatype, a description, and a default that matches the most common choice.
2. Add the computed symbols that express the resulting conditions in plain terms.
3. Tailor the content through source modifiers and inline preprocessor regions so every combination of options produces a valid project.
4. Expose the option to both entry points through the IDE and CLI host files.
5. Mirror the change in the matching sample so the sample stays a faithful skeleton.
6. Extend the wiki and the changelog. See [Documentation](./documentation.md).

The build generates a project for every combination in the option matrix and compiles it. An option that produces a combination the build cannot compile is a defect. See the validation section below.

## The SDK Build Flow

Scaffolded projects set `Sdk="Nice3point.Revit.Sdk"` and carry almost no build logic of their own. The SDK resolves the rest from the active configuration.

* **Configuration parsing.** The configuration name encodes the Revit version as a trailing number. The SDK extracts it, normalizes a two-digit form to the full year, and exposes it as the Revit version property.
* **Target framework selection.** The SDK maps the Revit version to the framework that version of Revit runs on, so a single project targets the correct framework per configuration.
* **Version constants.** A task derives the `REVIT####` and `REVIT####_OR_GREATER` constants for the active version from the full configuration list, so template content can gate version-specific Revit APIs.
* **Implicit usings.** A task adds the implicit `using` directives whose required assembly or global reference is present, so a project that references a given Revit assembly gets its matching namespaces without restating them.
* **Manifest patching.** A task adjusts the add-in manifest for the target version so a single manifest serves every supported Revit release.
* **Publishing and repacking.** Targets deploy the add-in for local debugging and, where enabled, merge dependencies and lay out the distribution for the installer and the store bundle.

Author build logic once in the SDK. When a step needs more than MSBuild expresses, add a task class to the SDK assembly and invoke it from a target. Keep the props and targets the contract, and treat the SDK properties a project sets as public surface. See [Code Style](./code-style.md).

## How Samples Mirror Templates

The samples are the templates' output made concrete. Each sample reflects one template option set, from a single-project add-in to a full multi-project solution, so a reader sees how a chosen combination looks once scaffolded. A sample references the SDK by published version where a template references it by name, which is the one expected difference between a sample and its generated counterpart.

Keep a sample aligned with its template. A template change that alters the output updates the sample in the same commit.

## Validation

The build verifies the templates by scaffolding and compiling them. It packs both packages to a local feed, installs the template package, generates a project for every combination in the option matrix, and builds each generated project against its first release configuration. A solution template additionally scaffolds its sub-projects before the build.

* Every change builds the affected templates and samples.
* When an option adds a new combination, confirm the build matrix covers it so the new path compiles.
* Run the build from the `build` directory as described in [AGENTS](../AGENTS.md).
