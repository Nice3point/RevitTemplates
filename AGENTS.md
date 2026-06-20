# Nice3point.Revit.Templates Agent Instructions

Nice3point.Revit.Templates is a public NuGet package of `dotnet new` project templates for Revit add-ins, paired with a custom MSBuild SDK. `source/Nice3point.Revit.Templates` holds the template package consumers install, `source/Nice3point.Revit.Sdk` holds the `Nice3point.Revit.Sdk` build SDK that template output depends on, and `samples/` holds runnable skeletons that mirror the templates.

## Non-Negotiables

* **Templates define what a consumer scaffolds.** A template is a real project annotated with `template.json` symbols and conditional content. Add or change a feature in the template content, never with a generator that runs outside the template engine.
* **The SDK owns the multi-version build.** Configuration parsing, target-framework selection, implicit usings, manifest patching, and publishing live in the SDK, never duplicated into template content.
* **Templates and samples stay in sync.** Each sample mirrors a template option set. A template change updates the matching sample so the sample stays a faithful skeleton.
* **The public surface is a contract.** Template short names, option names and values, SDK properties, and the `Nice3point.Revit.Sdk` package id are stable. Mark public SDK task classes `[PublicAPI]`. Rename only through a deprecation path.
* **Every supported Revit version compiles.** The SDK derives the target framework and version constants from the active configuration. Template content gates version-specific Revit APIs with `#if REVIT2024_OR_GREATER`-style directives.
* **The SDK loads under every build host.** The task assembly multi-targets so it loads under MSBuild on .NET Framework and on .NET, and the props and targets order around the standard build rather than fight it.
* **Templates are validated by building the samples.** Every change builds the affected templates and samples, since the build generates projects across the option matrix and compiles them. See [Templates](./docs/templates.md).
* **Verify unfamiliar APIs.** When unsure of a Revit, MSBuild, or .NET API's behavior or signature, confirm it before use. Search the web for the official docs. To read a referenced library's source, query GitHub with `gh` (`gh api`, `gh search code`). If `gh` is unavailable, search the web or ask. Never inspect compiled DLLs or XML extracted from NuGet packages.
* **Keep docs in sync.** A consumer-facing change updates `README.md`, `CHANGELOG.md`, and the wiki in the same commit. See [Documentation](./docs/documentation.md).

## Build

The build is a ModularPipelines project. Run `dotnet run -c Release` from the `build` directory to compile.

## Specialized Docs

Read the matching file before related work.

* [Project Structure](./docs/project-structure.md). The template package, the SDK, and the samples, and where each change belongs.
* [Templates & SDK](./docs/templates.md). The template engine model, the SDK build flow, and how samples mirror templates.
* [Code Style](./docs/code-style.md). C# conventions for the SDK, the build, and C# template content.
* [Documentation](./docs/documentation.md). README, CHANGELOG, the wiki, and XML doc rules.
* [Package Management](./docs/package-management.md). The per-project dependency model and Renovate.
