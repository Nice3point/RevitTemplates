The entire release process in different configurations is available in the cloud, you no longer need to manually carry out routine operations to deliver a product
to your users. 
The templates contain customized workflows for some popular pipelines.

## Table of content

<!-- TOC -->
* [Publishing Releases](#publishing-releases)
  * [Updating the Changelog](#updating-the-changelog)
  * [Creating a new Release from the JetBrains Rider](#creating-a-new-release-from-the-jetbrains-rider)
  * [Creating a new Release from the Terminal](#creating-a-new-release-from-the-terminal)
  * [Creating a new Release on GitHub](#creating-a-new-release-on-github)
  * [Creating a new release on Azure DevOps](#creating-a-new-release-on-azure-devops)
* [Dependency conflicts](#dependency-conflicts)
  * [Dependency isolation](#dependency-isolation)
  * [Dependency repacking](#dependency-repacking)
<!-- TOC -->

## Publishing Releases

Releases are managed by creating new [Git tags](https://git-scm.com/book/en/v2/Git-Basics-Tagging).
A tag in Git used to capture a snapshot of the project at a particular point in time, with the ability to roll back to a previous version.

Tags must follow the format `version` or `version-stage.n.date` for pre-releases, where:

- **version** specifies the version of the release:
    - `1.0.0`
    - `2.3.0`
- **stage** specifies the release stage:
    - `alpha` - represents early iterations that may be unstable or incomplete.
    - `beta` - represents a feature-complete version but may still contain some bugs.
- **n** prerelease increment (optional):
    - `1` - first alpha prerelease
    - `2` - second alpha prerelease
- **date** specifies the date of the pre-release (optional):
    - `250101`
    - `20250101`

For example:

| Stage   | Version                |
|---------|------------------------|
| Alpha   | 1.0.0-alpha            |
| Alpha   | 1.0.0-alpha.1.20250101 |
| Beta    | 1.0.0-beta.2.20250101  |
| Release | 1.0.0                  |

### Updating the Changelog

For releases, changelog for the release version is required.

To update the changelog:

1. Navigate to the solution root.
2. Open the file **Changelog.md**.
3. Add a section for your version. The version separator is the `#` symbol.
4. Specify the release number e.g. `# 1.0.0` or `# 25.01.01 v1.0.0`, the format does not matter, the main thing is that it contains the version.
5. In the lines below, write a changelog for your version, style to your taste. For example, you will find changelog for version 1.0.0, do the same.
6. Make a commit.

### Creating a new Release from the JetBrains Rider

JetBrains provides a handy UI for creating a tag, it can be created in a few steps:

1. Open JetBrains Rider.
2. Navigate to the **Git** tab.
3. Click **New Tag...** and create a new tag.

   ![image](https://github.com/user-attachments/assets/19c11322-9f95-45e5-8fe6-defa36af59c4)

4. Navigate to the **Git** panel.
5. Expand the **Tags** section.
6. Right-click on the newly created tag and select **Push to origin**.

   ![image](https://github.com/user-attachments/assets/b2349264-dd76-4c21-b596-93110f1f16cb)

   This process will trigger the Release workflow and create a new Release on GitHub.

### Creating a new Release from the Terminal

Alternatively, you can create and push tags using the terminal:

1. Navigate to the repository root and open the terminal.
2. Use the following command to create a new tag:
   ```shell
   git tag 'version'
   ```

   Replace `version` with the desired version, e.g., `1.0.0`.
3. Push the newly created tag to the remote repository using the following command:
   ```shell
   git push origin 'version'
   ```

> [!NOTE]  
> The tag will reference your current commit, so verify you're on the correct branch and have fetched latest changes from remote first.

### Creating a new Release on GitHub

To create releases directly on GitHub:

1. Navigate to the **Actions** section on the repository page.
2. Select **Publish Release** workflow.
3. Click **Run workflow** button.
4. Specify the release version and click **Run**.

   ![image](https://github.com/user-attachments/assets/088388c1-6055-4d21-8d22-70f047d8f104)

> [!IMPORTANT]  
> Set write permissions in the repository settings, this is a prerequisite for publishing a release.
> 
> ![image](https://github.com/user-attachments/assets/2f1a37dc-d870-4d0d-949e-b5c8e2c34e57)  

### Creating a new release on Azure DevOps

To create releases directly on Azure:

1. Navigate to the **Pipelines** section on the project page.
2. Click **New pipeline** button and select the source repository.
3. Save your pipeline.
4. Click **Run pipeline** button.
5. Specify the release version and click **Run**.

   ![image](https://github.com/user-attachments/assets/39d2b173-3092-48a5-a9eb-7c0981708b07)

> [!IMPORTANT]  
> Set write permissions in the repository settings, this is a prerequisite for publishing a release.
> 
> ![image](https://github.com/user-attachments/assets/378d357a-d99d-4094-a8ca-834babab5554)

## Dependency conflicts

Plugins often use third-party dependencies and plugins from other developers may use these dependencies, but of different versions, causing Revit to conflict and crash.
There is no way to control this process, so there are several ways to resolve this issue:

### Dependency isolation

Starting with Revit version 2026, it provides dependency isolation in a separate [context](https://learn.microsoft.com/en-us/dotnet/core/dependency-loading/understanding-assemblyloadcontext).
This helps you to use any version of dependencies, in the same domain.

To enable it, modify the `.addin` manifest:

```xml
<RevitAddIns>
    <AddIn Type="Application">
        <Name>RevitAddin</Name>
        <Assembly>RevitAddin\RevitAddin.dll</Assembly>
    </AddIn>
    <ManifestSettings>
        <UseRevitContext>False</UseRevitContext>
        <ContextName>RevitAddin</ContextName>
    </ManifestSettings>
</RevitAddIns>
```

> [!NOTE]  
> Adding `ManifestSettings` to the manifest for Revit earlier than 2026, will cause a crash.
> [Revit.Build.Tasks](https://github.com/Nice3point/Revit.Build.Tasks) provides a fix for this issue during project building.

### Dependency repacking

For older Revit versions before 2026, you can use ILRepack, a tool that repacks multiple DLLs into one.
It does not support repackaging C++ dependencies, and may be unstable in some complex scenarios, so it is not recommended, but it is the only solution.

To enable it, set the `IsRepackable` property to `true`.
For C++ dependencies or dependencies that cause issues, add to the `RepackBinariesExcludes` property:

```xml
<PropertyGroup>
    <IsRepackable Condition="'$(RevitVersion)' &lt; '2026'">true</IsRepackable>
    <RepackBinariesExcludes>$(AssemblyName).UI.dll;System*.dll</RepackBinariesExcludes>
</PropertyGroup>
```

> [!NOTE]  
> For repacking, third-party packages are used.
> You can find more detailed documentation about it here
> [Revit.Build.Tasks](https://github.com/Nice3point/Revit.Build.Tasks) and here [ILRepack](https://www.nuget.org/packages/ILRepack).