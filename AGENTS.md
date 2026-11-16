# Nice3point.Revit.Templates

Nice3point.Revit.Templates ships as NuGet packages: the set of `dotnet new` project templates for creating Revit add-ins, a custom MSBuild SDK for building them.
Each template is a real project annotated for the .NET template engine; the SDK owns the multi-version build, and a scaffolded project file stays almost empty.

## Non-negotiables

The installed agent skills define the C# style, the XML documentation, the logging, the options validation, the JSON serialization, and the writing standard.
The rules below add the specifics of this repository.

* A feature a consumer scaffolds is authored in the template content through `template.json` symbols and conditional content, never through a generator that runs outside the template engine.
* The SDK owns the multi-version build. Configuration parsing, target-framework selection, implicit usings, manifest patching, and publishing live in the SDK, never duplicated into template content.
* Templates and samples stay in sync. A template change that alters the output updates the matching sample in the same commit. A sample references the SDK by published version where a template references it by name.
* The public surface is a contract. Template short names, option names and values, SDK properties, and the `Nice3point.Revit.Sdk` package id are stable; rename only through a deprecation path.
* An MSBuild task returns, never throws. It reports failure through the task log and a `false` result, and returns `true` as a no-op when nothing applies.
* The task assembly multi-targets to load under MSBuild on .NET Framework, older .NET Core hosts, and the current .NET SDK. The SDK props and targets hook the standard build with `AfterTargets`/`BeforeTargets`, never by replacing a built-in target.
* Template content holds to the same code bar as shipped code; generated code is the consumer's first impression.
* A consumer-facing change updates `README.md`, `CHANGELOG.md`, and the wiki in the same commit.

## Template content

* Generated code is a starting point the consumer extends. An extension point stays in place even where the IDE offers a shorter form: the `if` around an optional `StringBuilder`, the `{ }` body of an empty view model.
* The pipeline steps of a build project are in `Pipeline/`, grouped by capability: `Versioning/`, `Compilation/`, `Testing/`, `Packaging/`, and `Publishing/`. An options type and its generated validator are beside the steps that read them, and the options several capabilities share are at the root of `Pipeline/`.
* A tool ModularPipelines has no package for has a folder of its own at the root of the build project, named after the tool, such as `Wix/`. The folder has a subfolder per command group, and a command is a method of its group: `wix extension add` is `context.Tools.Wix.Extension.AddAsync`.
* `Program.cs` binds and validates the options of the build; a build project has no registration extension.
* A Shouldly assertion message of the build and the text of the installer user interface are public messages. The text of the installer user interface uses sentence case: `Revit add-in`, `Installs the add-in for Revit 2026.`
* The installer project declares the manifest contract and its JSON context in `installer/Manifest/`, and the build project compiles the same files as links. The installer project disables reflection-based JSON serialization.
* The build projects keep reflection-based JSON serialization enabled: ModularPipelines.Git deserializes the GitVersion output through reflection. The JSON a build project owns is serialized through a source-generated context.
* A build module is self-contained. Two modules may repeat a small helper, such as `VersionRegex`, and template content has no shared helper file.
* `samples/MultiProjectSolution/build` and `samples/MultiProjectSolution/installer` equal the Solution template with every option enabled; only the Sourcy solution name differs (`Solutions.MultiProjectSolution`).
* Template content and samples use LF line endings and end with a newline. The package packs the files from the working tree, and a CRLF file on disk reaches the consumer.
* Renovate skips a file that holds template directives, such as the `global.json` of the Solution template. Such a file is updated by hand together with its counterpart at the root.

## CHANGELOG

A release entry is written for the consumer, in the format of the RevitToolkit changelog.

* The entry opens with one or two sentences on the main changes and a link to the migration guide when one exists.
* `## Templates` groups the changes under `###` topics ordered by impact. A headline feature gets a short description and a code example; minor changes and fixes stand as bullets in the topics at the end, with `Updated dependencies.` last.
* `## SDK` follows `## Templates`.
* A release with a breaking change adds `## Breaking changes` and `## Migration guide`, with a `###` section per project kind and `// Before` and `// After` code.

## Package management

The solution pins package versions per project; there is no central `Directory.Packages.props`.

* Each project — the SDK, the build, and template content — declares a concrete version on every `PackageReference`.
* The Revit API and companion Revit packages float to `$(RevitVersion).*`; they enter through the SDK and template content, never a shared central file.
* Keep the SDK and template content dependency-light; mark a build-time-only dependency `PrivateAssets="all"`.

## Repository map

* `source/Nice3point.Revit.Templates/` — the `dotnet new` template package a consumer installs. Each template is a self-compiling project; the package packs the template content as NuGet content.
* `source/Nice3point.Revit.Sdk/` — the custom MSBuild SDK. `Sdk/` holds the props and targets contract; the MSBuild task classes sit at the project root.
* `samples/` — runnable add-in skeletons, one per template option set, referencing the published SDK by version.
* `wiki/` — the consumer-facing wiki source.
* `build/` — the ModularPipelines build.
* Root — build and package configuration, `README.md`, `CHANGELOG.md`, the license, CI workflows.

## Build and verify

* Build the SDK: `dotnet build source/Nice3point.Revit.Sdk -c Release`.
* Build a sample: `dotnet build samples/<sample> -c Release.R##`, where the `R##` suffix is the Revit year (`R27` targets Revit 2027). A sample mirrors a template; a green sample build validates the equivalent scaffolded output.
* Verify an option set that no sample covers by scaffolding it from an isolated hive:
  `dotnet new install source/Nice3point.Revit.Templates/<template> --debug:custom-hive <hive>`, then `dotnet new <short name> <options> --debug:custom-hive <hive>`.
  Scaffold outside any directory with a `Directory.Packages.props`, and pin the SDK in the generated project as `Nice3point.Revit.Sdk/<published version>`.
