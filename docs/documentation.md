# Documentation

These rules govern every piece of prose the project ships: XML doc comments, `README.md`, `CHANGELOG.md`, and the wiki. Each format adds its own rules on top of the shared set.

A consumer-facing change updates the README, the CHANGELOG, the wiki, and any affected XML docs in the same commit. Documentation that lags the code is a defect.

## Shared Prose Rules

* **State what, not how.** Describe observable behavior and contract, never the implementation. A summary survives an implementation rewrite unchanged.
* **Plain technical English.** No corporate jargon, no marketing tone.
* **No filler.** Omit obvious statements. State only what a reader cannot infer from the signature or the option name.
* **Third-person present indicative.** Write "Patches the manifest", not "Patching the manifest". No `-ing` verb form for what a member does.
* **One sentence per line.** Break at sentence boundaries, never at a fixed character width.
* **No dashes or semicolons.** Use separate sentences or commas.

## XML Doc Comments

The SDK ships public task classes, so they carry documentation.

* Document every public member with a `<summary>` that states what it does.
* **`<summary>` describes the member, not its parameters.** Parameters belong in `<param>`, the return value in `<returns>`. Do not restate the signature in prose.
* Add `<remarks>` for a non-trivial constraint, such as an ordering requirement against the standard build.
* Reference another type or member with `<see cref="..."/>` so renames stay tracked.

## README

The README introduces the package and links the wiki for depth. Keep the feature list aligned with what the templates and the SDK actually offer, and keep the install and usage commands current. The packed README drops the logo banner during the build, so keep the banner self-contained at the top.

## CHANGELOG

Update the current preview or release version section. Group changes by the surface they touch under a `## Templates` or `## SDK` heading, matching the existing format. Categorize every change, not only the major ones, and reference the issue or pull request where one exists.

* New options, templates, SDK properties, and build steps.
* Breaking changes such as a renamed option, short name, or SDK property.
* Improvements and dependency bumps.
* Bug fixes.

Provide a migration note for any breaking change.

## Wiki

The wiki is the consumer-facing guide to using the templates and the SDK. Its source lives under `wiki` and publishes to the project wiki on change. The wiki covers how to install the templates, choose options, build, and publish, organized as the home page and sidebar present it.

* Add or revise the wiki page that owns a consumer-facing change.
* Keep a wiki page focused on the consumer's task, not the internal authoring detail that lives in the agent guidelines.
* The SDK package README is sourced from the SDK wiki page, so a change there reaches the package listing.

## Guidelines and wiki

The agent guidelines live under `docs` and the wiki source under `wiki`. They serve different readers. The guidelines describe how to author the package. The wiki describes how to consume it. Keep authoring detail out of the wiki and consumer instructions out of the guidelines.
