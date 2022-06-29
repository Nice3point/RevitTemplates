using Autodesk.Revit.UI;
using Nice3point.Revit.AddIn.Commands;
<!--#if (Logger)
using Serilog.Events;
#endif-->

namespace Nice3point.Revit.AddIn;

[UsedImplicitly]
public class Application : IExternalApplication
{
    public Result OnStartup(UIControlledApplication application)
    {
<!--#if (Logger)
        CreateLogger();
#endif-->
        CreateRibbon(application);
<!--#if (!NoWindow)
        ForceLoadLibraries();
#endif-->
        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
<!--#if (Logger)
        Log.CloseAndFlush();
#endif-->
        return Result.Succeeded;
    }

    private static void CreateRibbon(UIControlledApplication application)
    {
        var panel = application.CreatePanel("Panel name", "Nice3point.Revit.AddIn");

        var showButton = panel.AddPushButton<Command>("Button text");
        showButton.SetImage("/Nice3point.Revit.AddIn;component/Resources/Icons/RibbonIcon16.png");
        showButton.SetLargeImage("/Nice3point.Revit.AddIn;component/Resources/Icons/RibbonIcon32.png");
    }
<!--#if (!NoWindow)

    /// <summary>
    ///     Forced loading of libraries into the project. Typically used for XAML related libraries
    /// </summary>
    private static void ForceLoadLibraries()
    {
        var assemblies = new List<string>
        {
            "Microsoft.Xaml.Behaviors"
        };

        foreach (var assembly in assemblies) AppDomain.CurrentDomain.Load(assembly);
    }
#endif-->
<!--#if (Logger)

    /// <summary>
    ///     Globally-shared logger. To connect new sinks install additional nuget packages
    /// </summary>
    private static void CreateLogger()
    {
        const string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Debug(LogEventLevel.Debug, outputTemplate)
            .MinimumLevel.Debug()
            .CreateLogger();

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            var e = (Exception) args.ExceptionObject;
            Log.Fatal(e, "Domain unhandled exception");
        };
    }
#endif-->
}