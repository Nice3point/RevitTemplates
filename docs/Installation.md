# Install .Net SDK

RevitTemplates uses the [.Net Template Engine](https://github.com/dotnet/templating/wiki) and you need to install .Net on your computer. 

If you don't have it, you can find the latest version here: https://dotnet.microsoft.com/download

# Install Package

The templates are supplied as a Nuget package https://www.nuget.org/packages/Nice3point.Revit.Templates

To install it, open terminal, paste the command and press **Enter**:

```shell
dotnet new install Nice3point.Revit.Templates
```

After installation, templates will be displayed in your IDE.

# Install Nuke Global Tool

For enterprise development, you might need an additional tool.
If you need to create an installer for the add-in on your local computer, enter the following command:

```shell
dotnet tool install Nuke.GlobalTool --global
```

> [!NOTE]  
> If you plan to build only on the GitHub or any other remote server, you do not need this command.