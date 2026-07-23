# Nice3point.Revit.Templates

Nice3point.Revit.Templates ships as NuGet packages: the set of `dotnet new` project templates for creating Revit add-ins, a custom MSBuild SDK for building them.
Each template is a real project annotated for the .NET template engine; the SDK owns the multi-version build, and a scaffolded project file stays almost empty.

## Non-negotiables

* A feature a consumer scaffolds is authored in the template content through `template.json` symbols and conditional content, never through a generator that runs outside the template engine.
* The SDK owns the multi-version build. Configuration parsing, target-framework selection, implicit usings, manifest patching, and publishing live in the SDK, never duplicated into template content.
* Templates and samples stay in sync. A template change that alters the output updates the matching sample in the same commit. A sample references the SDK by published version where a template references it by name.
* The public surface is a contract. Template short names, option names and values, SDK properties, and the `Nice3point.Revit.Sdk` package id are stable; rename only through a deprecation path.
* An MSBuild task returns, never throws. It reports failure through the task log and a `false` result, and returns `true` as a no-op when nothing applies.
* The task assembly multi-targets to load under MSBuild on .NET Framework, older .NET Core hosts, and the current .NET SDK. The SDK props and targets hook the standard build with `AfterTargets`/`BeforeTargets`, never by replacing a built-in target.
* Template content holds to the same code bar as shipped code; generated code is the consumer's first impression.
* Confirm an unfamiliar Revit, MSBuild, or .NET API before use through official docs or `gh` (`gh api`, `gh search code`).
* A consumer-facing change updates `README.md`, `CHANGELOG.md`, and the wiki in the same commit. Group CHANGELOG entries under `## Templates` or `## SDK` and add a migration note for a breaking change.

## Repository map

* `source/Nice3point.Revit.Templates/` — the `dotnet new` template package a consumer installs. Each template is a self-compiling project; the package packs the template content as NuGet content.
* `source/Nice3point.Revit.Sdk/` — the custom MSBuild SDK. `Sdk/` holds the props and targets contract; the MSBuild task classes sit at the project root.
* `samples/` — runnable add-in skeletons, one per template option set, referencing the published SDK by version.
* `wiki/` — the consumer-facing wiki source.
* `build/` — the ModularPipelines build.
* Root — build and package configuration, `README.md`, `CHANGELOG.md`, the license, CI workflows.

## Package management

The solution pins package versions per project; there is no central `Directory.Packages.props`.

* Each project — the SDK, the build, and template content — declares a concrete version on every `PackageReference`.
* The Revit API and companion Revit packages float to `$(RevitVersion).*`; they enter through the SDK and template content, never a shared central file.
* Keep the SDK and template content dependency-light; mark a build-time-only dependency `PrivateAssets="all"`.

## Build and verify

* Build the SDK: `dotnet build source/Nice3point.Revit.Sdk -c Release`.
* Build a sampl: `dotnet build samples/<sample> -c Release.R##`, where the `R##` suffix is the Revit year (`R27` targets Revit 2027). A sample mirrors a template; a green sample build validates the equivalent scaffolded output.
