using Autodesk.Revit.UI;
using Nice3point.Revit.AddIn.Commands;

namespace Nice3point.Revit.AddIn;

[UsedImplicitly]
public class Application : IExternalApplication
{
    public Result OnStartup(UIControlledApplication application)
    {
        ForceLoadLibraries();
        CreateRibbon(application);
        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        return Result.Succeeded;
    }

    private static void CreateRibbon(UIControlledApplication application)
    {
        var panel = application.CreatePanel("Panel name", "Nice3point.Revit.AddIn");

        var showButton = panel.AddPushButton<Command>("Button text");
        showButton.SetImage("/Nice3point.Revit.AddIn;component/Resources/Icons/RibbonIcon16.png");
        showButton.SetLargeImage("/Nice3point.Revit.AddIn;component/Resources/Icons/RibbonIcon32.png");
    }

    /// <summary>
    ///     Forced loading of libraries into the project. Typically used for XAML related libraries
    /// </summary>
    private static void ForceLoadLibraries()
    {
        var assemblies = new List<string>
        {
<!--#if (!NoWindow)
            "Microsoft.Xaml.Behaviors",
#endif-->
        };

        foreach (var assembly in assemblies) AppDomain.CurrentDomain.Load(assembly);
    }
}