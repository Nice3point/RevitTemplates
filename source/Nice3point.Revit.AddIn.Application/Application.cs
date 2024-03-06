using Nice3point.Revit.Toolkit.External;
using Nice3point.Revit.AddIn.Commands;
#if (Logger && !IOC)
using Serilog.Events;
#endif

namespace Nice3point.Revit.AddIn;

/// <summary>
///     Application entry point
/// </summary>
[UsedImplicitly]
public class Application : ExternalApplication
{
    public override void OnStartup()
    {
#if (IOC)
        Host.Start();
#endif
#if (Logger && !IOC)
        CreateLogger();
#endif
        CreateRibbon();
    }
#if (Hosting || (!IOC && Logger))

    public override void OnShutdown()
    {
#if (Hosting)
        Host.Stop();
#elseif (Logger)
        Log.CloseAndFlush();
#endif
    }
#endif

    private void CreateRibbon()
    {
        var panel = Application.CreatePanel("Commands", "Nice3point.Revit.AddIn");

        var showButton = panel.AddPushButton<StartupCommand>("Execute");
        showButton.SetImage("/Nice3point.Revit.AddIn;component/Resources/Icons/RibbonIcon16.png");
        showButton.SetLargeImage("/Nice3point.Revit.AddIn;component/Resources/Icons/RibbonIcon32.png");
    }
#if (Logger && !IOC)

    private static void CreateLogger()
    {
        const string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Debug(LogEventLevel.Debug, outputTemplate)
            .MinimumLevel.Debug()
            .CreateLogger();

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            var exception = (Exception)args.ExceptionObject;
            Log.Fatal(exception, "Domain unhandled exception");
        };
    }
#endif
}