# Project Structure

Nice3point.Revit.Templates ships two NuGet packages and a set of runnable skeletons. The template package scaffolds Revit add-in projects through the .NET template engine. The SDK package provides the MSBuild build that scaffolded projects depend on. The samples mirror the templates so each option set has a buildable reference. Keep each piece of code in the project that owns its responsibility.

## Solution Groups

* **`/source`**: the two shipped NuGet packages.
    * The template package is the set of `dotnet new` templates. Each template is a real project with a `.template.config` folder that declares its options and conditional content. The package itself produces no build output and packs the template content as NuGet content.
    * The SDK package is the custom MSBuild SDK that scaffolded projects reference. It carries the props and targets that drive the multi-version build, plus the MSBuild task assembly that backs them.
* **`/samples`**: runnable add-in skeletons that mirror the templates. Each sample reflects a template option set and references the published SDK by version. They are the reference for how generated output looks and builds.
* **`/build`**: the ModularPipelines build that compiles, packs both packages, generates projects from the templates across the option matrix, and publishes.
* **`/docs`**: the agent guidelines. See [Documentation](./documentation.md).
* **`/wiki`**: the consumer-facing wiki source, published to the project wiki on change.
* **Root**: build and package configuration, the README and CHANGELOG, the license, the agent guidelines, and the CI workflows.

## Change Placement

* A new scaffolding option or a change to conditional content goes in the matching template under `source/Nice3point.Revit.Templates`, declared through its `template.json`. See [Templates & SDK](./templates.md).
* A change to the multi-version build, the target-framework rules, the implicit usings, manifest patching, or publishing goes in the SDK props and targets under `source/Nice3point.Revit.Sdk/Sdk`.
* A new MSBuild build step that needs C# logic goes in the SDK task assembly, invoked from a target.
* A template change that affects the generated output updates the mirroring sample under `samples` so the sample stays faithful.
* A change to the publishing flow, the packing, or the template-generation check goes in the matching build module under `build/Modules`.
* Consumer-facing usage documentation goes in the wiki source under `wiki`, not in the agent guidelines.
