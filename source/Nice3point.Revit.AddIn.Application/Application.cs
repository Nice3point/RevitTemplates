using Nice3point.Revit.Toolkit.External;
using Nice3point.Revit.AddIn.Commands;
#if (log && !UseIoc)
using Serilog;
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
#if (UseIoc)
        Host.Start();
#endif
#if (log && !UseIoc)
        CreateLogger();
#endif
        CreateRibbon();
    }
#if (Hosting || (!UseIoc && log))

    public override void OnShutdown()
    {
#if (Hosting)
        Host.Stop();
#elseif (log)
        Log.CloseAndFlush();
#endif
    }
#endif

    private void CreateRibbon()
    {
        var panel = Application.CreatePanel("Commands", "Nice3point.Revit.AddIn");

        panel.AddPushButton<StartupCommand>("Execute")
            .SetImage("/Nice3point.Revit.AddIn;component/Resources/Icons/RibbonIcon16.png")
            .SetLargeImage("/Nice3point.Revit.AddIn;component/Resources/Icons/RibbonIcon32.png");
    }
#if (log && !UseIoc)

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