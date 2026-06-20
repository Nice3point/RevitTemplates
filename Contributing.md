# Contributing to Nice3point.Revit.Templates

Thanks for taking the time to contribute. This guide covers issues and pull requests. For the architecture and conventions, see the project guidelines in [AGENTS.md](AGENTS.md) and the [docs](docs/) folder.

## Issues

* Search the existing issues and discussions before you open a new one.
* For a bug, describe what you expected, what happened, and the smallest steps that reproduce it. Include the template package version and the Revit version you target.
* For a feature, describe the problem it solves, not only the solution you have in mind.
* For a large or breaking change, open an issue first so the approach is agreed before you write code.

## Pull Requests

* Keep each pull request focused on one concern. Split unrelated changes into separate pull requests.
* Fork the repository, branch from the default branch, and open a draft pull request early.
* Match the style and patterns of the surrounding code.
* Validate a template change by generating a project from the affected template and building it.
* Update the README, the CHANGELOG, and the wiki in the same pull request as any consumer-facing change.
* Write a clear title and description, and link the issue the pull request resolves.
* Make sure the build passes before you mark the pull request ready for review.

## Development

Run `dotnet run` from the `build` directory to compile. The SDK version is pinned in `global.json`.

Please keep issues and pull requests respectful and on topic.
