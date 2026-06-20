# Code Style

Production C# only. The same conventions cover the SDK task assembly, the build, and the C# in template content, since scaffolded code becomes a consumer's starting point and sets their habits.

## General Principles

* **SOLID and DRY.** One responsibility per type. Extract shared logic rather than duplicate it.
* **Explicit over implicit.** Code is self-explanatory. Avoid hidden behavior and unclear defaults.
* **Nullable safety.** Nullable reference types are enabled. Treat every nullability warning as a defect.
* **MSBuild tasks return, not throw.** An SDK task reports failure through the task log and a `false` result, so a build step fails cleanly rather than crashing the build host.
* **StyleCop style.** Follow StyleCop conventions for layout, member ordering, and spacing.

## Modern C#

`LangVersion` is `latest`. Reach for the newest feature that expresses the intent directly, and do not hand-roll what the language already provides.

* Primary constructors when a type captures state.
* Collection expressions for literals and spans.
* Pattern matching and switch expressions over branching chains.
* Expression-bodied members for simple accessors.
* File-scoped namespaces.

Template content holds to the same bar. Generated code is the consumer's first impression of the project, so it stays idiomatic and current.

## Comments

Comments are the exception inside the code.

* Names and structure carry the meaning. Default to no comment.
* Add one only when the reason cannot be read from the code and a reader could break the code without it, such as a non-obvious MSBuild ordering constraint.
* A comment explains why, never what. Do not restate the code.
* In a props or targets file, a banner comment names a section so the build reads as a sequence of stages.

## Attributes

Decorate members with every JetBrains and .NET attribute that carries meaning, so analyzers, the debugger, and callers read the full contract.

* `[PublicAPI]` on every public SDK task class.
* `[Required]` on a task input MSBuild must supply, and `[Output]` on a value a task returns to the build.
* `[Pure]` on a read-only method.

## Naming

* **Clarity first.** Names are descriptive and never abbreviated: `configuration` not `config`, `version` not `ver`, `reference` not `ref`.
* An MSBuild task and its target read as the step they perform.
* A template symbol name states the option it controls, and a computed symbol reads as the condition it expresses.
* No single-letter variables except in a short loop or lambda.

## File and Class Structure

* **File-scoped namespaces** throughout the SDK and the build.
* **Member order:** private fields, constructors, public properties, public methods, private methods.
