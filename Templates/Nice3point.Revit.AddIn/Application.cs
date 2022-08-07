using Nice3point.Revit.Toolkit.External;
using Nice3point.Revit.AddIn.Commands;
<!--#if (Logger)
using Serilog.Events;
#endif-->

namespace Nice3point.Revit.AddIn;

[UsedImplicitly]
public class Application : ExternalApplication
{
    public override void OnStartup()
    {
<!--#if (Logger)
        CreateLogger();
#endif-->
        CreateRibbon();
    }
<!--#if (Logger)

    public override void OnShutdown()
    {
        Log.CloseAndFlush();
    }
#endif-->

    private void CreateRibbon()
    {
        var panel = Application.CreatePanel("Panel name", "Nice3point.Revit.AddIn");

        var showButton = panel.AddPushButton<Command>("Button text");
        showButton.SetImage("/Nice3point.Revit.AddIn;component/Resources/Icons/RibbonIcon16.png");
        showButton.SetLargeImage("/Nice3point.Revit.AddIn;component/Resources/Icons/RibbonIcon32.png");
    }
<!--#if (Logger)

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