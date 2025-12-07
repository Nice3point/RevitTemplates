By default, the `.dll` and `.addin` files are included for publishing.
If you need to include additional files, such as configurations or family files, include them in the `Content` item.

## Tagging third-party files

All extra files that will be copied to the Revit Add-ins folder and also added to the installer must be tagged.
First you need to change the `build action` for the file from `None` to `Content`

```xml
<ItemGroup>
    <Content Include="Resources\Families\Window.rfa"/>
    <Content Include="Resources\Music\Click.wav"/>
    <Content Include="Resources\Images\Image.png"/>
    <Content Include="Readme.md"/>
</ItemGroup>
```

The `PublishDirectory` property specifies which subfolder of the plugin the file should be copied to.
If it is not specified, the files will be copied to the root add-in folder.

```xml
<ItemGroup>
    <Content Include="Resources\Families\Window.rfa" PublishDirectory="Families"/>
    <Content Include="Resources\Music\Click.wav" PublishDirectory="Music\Effects"/>
    <Content Include="Resources\Images\Image.png" PublishDirectory="Images"/>
    <Content Include="Readme.md"/>
</ItemGroup>
```

To disable copying Content file, set `CopyToPublishDirectory="Never"`

```xml
<ItemGroup>
    <Content Include="Contributing.md" CopyToPublishDirectory="Never"/>
</ItemGroup>
```

Remember to use [wildcards](https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-select-the-files-to-build) when adding files with similar names:

```xml
<ItemGroup>
    <Content Include="Resources\Families\*"/>
    <Content Include="Resources\Music\*.wav"/>
    <Content Include="Resources\**\*"/>
    <Content Include="Resources\**\*.png"/>
</ItemGroup>
```

- `Resources\Families\*` - all files in the `Families` folder inside the `Resources` folder.
- `Resources\Music\*.wav` - all files with the extension `.wav` in the `Music` folder, which is located in the `Resources` folder.
- `Resources\**\**` - all files in the `Resources` folder and all its subfolders.
- `Resources\**\*\*.png` - all files with extension `.png` in the `Resources` folder and all its subfolders.

## Publishing files

Publishing is used to create a package of files used by the installers, as well as for local debugging of your add-in.

> [!NOTE]  
> For publishing, a third-party package is used.
> You can find more detailed documentation about it here: [Revit.Build.Tasks](https://github.com/Nice3point/Revit.Build.Tasks)

Publishing files disabled by default.
To enable it, set `<PublishRevitAddin>true</PublishRevitAddin>` or `<DeployRevitAddin>true</DeployRevitAddin>`.

> [!IMPORTANT]  
> Publishing should only be enabled in the main project containing the `.addin` file.

## Result

```text
📂%AppData%\Autodesk\Revit\Addins\2025
 ┣📜RevitAddIn.addin
 ┗📂RevitAddIn
   ┣📂Families
   ┃ ┗📜Family.rfa
   ┣📂Images
   ┃ ┣📜Image.png
   ┃ ┣📜Image2.png
   ┃ ┗📜Image3.jpg
   ┣📂Music
   ┃ ┗📂Effects
   ┃   ┗📜Click.wav
   ┣📜CommunityToolkit.Mvvm.dll
   ┣📜RevitAddIn.dll
   ┗📜Readme.md
```