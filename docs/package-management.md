# Package Management

The solution pins package versions per project. There is no central `Directory.Packages.props`. Each project declares its own versions on the `PackageReference` item. Renovate (`renovate.json`) bumps versions automatically against the `develop` branch, so manual version edits are rare.

## The Per-Project Model

* The SDK project and the build project pin a concrete version on each `PackageReference`.
* Template content pins a concrete version on each `PackageReference` so a scaffolded project starts from a known-good set. The Revit API and the companion Revit packages float to `$(RevitVersion).*` so a project resolves the right package for the configuration it builds.
* A sample pins the same versions as the template it mirrors, and references the SDK by its published version.
* The Revit API packages enter through the SDK and template content, never through a shared central file.

## Add a Dependency

1. Add a `PackageReference` with a concrete version to the project that uses it.
2. Keep the scope narrow. The SDK and the template content stay dependency-light, so prefer a platform, MSBuild, or Revit API before introducing a new package.
3. Where a dependency is build-time only and must not flow to a consumer, mark it `PrivateAssets="all"`.
4. When the dependency varies by Revit version, float it to `$(RevitVersion).*` rather than pin a single version.
