using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
#if (!useUi)
using Autodesk.Revit.UI;
#endif
#if (useUi && !useDi)
using Nice3point.Revit.AddIn._1.ViewModels;
#endif
#if (useUi)
using Nice3point.Revit.AddIn._1.Views;
#endif
#if (addinLogging && isCommandAddin && !useDi)
using Serilog;
using Serilog.Events;
#endif

namespace Nice3point.Revit.AddIn._1.Commands;

/// <summary>
///     External command entry point.
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
#if (diHosting && isCommandAddin)
public class StartupCommand : AsyncExternalCommand
{
    public override async Task ExecuteAsync()
    {
        await Host.StartAsync();
#if (useUi)
        var view = Host.GetService<Nice3point_Revit_AddIn__1View>();
        view.ShowDialog();
#elseif (!useUi)
        TaskDialog.Show(Document.Title, "Nice3point.Revit.AddIn.1");
#endif
    }
}
#else
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
#if (useDi && isCommandAddin)
        Host.Start();
#endif
#if (addinLogging && isCommandAddin && !useDi)
        var logger = CreateLogger();
#endif
#if (useUi && useDi)
        var view = Host.GetService<Nice3point_Revit_AddIn__1View>();
        view.ShowDialog();
#elseif (!useUi && useDi)
        TaskDialog.Show(Document.Title, "Nice3point.Revit.AddIn.1");
#elseif (useUi)
        var viewModel = new Nice3point_Revit_AddIn__1ViewModel();
        var view = new Nice3point_Revit_AddIn__1View(viewModel);
        view.ShowDialog();
#elseif (!useUi)
        TaskDialog.Show(Document.Title, "Nice3point.Revit.AddIn.1");
#endif
    }
#if (addinLogging && isCommandAddin && !useDi)

    private static ILogger CreateLogger()
    {
        const string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

        return new LoggerConfiguration()
            .WriteTo.Debug(LogEventLevel.Debug, outputTemplate)
            .MinimumLevel.Debug()
            .CreateLogger();
    }
#endif
}
#endif