using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
#if (!useUi)
using Autodesk.Revit.UI;
#endif
#if (useUi && !useDi)
using Nice3point.Revit.AddIn.ViewModels;
#endif
#if (useUi)
using Nice3point.Revit.AddIn.Views;
#endif
#if (addinLogging && isCommandAddin && !useDi)
using Serilog;
using Serilog.Events;
#endif

namespace Nice3point.Revit.AddIn.Commands;

/// <summary>
///     External command entry point.
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
#if (addinLogging && isCommandAddin && !useDi)
        var logger = CreateLogger();
#endif
#if (useUi && useDi)
        var view = Host.GetService<Nice3point.Revit.AddInView>();
        view.ShowDialog();
#elseif (!useUi && useDi)
        TaskDialog.Show(Document.Title, "Nice3point.Revit.AddIn");
#elseif (useUi)
        var viewModel = new Nice3point.Revit.AddInViewModel();
        var view = new Nice3point.Revit.AddInView(viewModel);
        view.ShowDialog();
#elseif (!useUi)
        TaskDialog.Show(Document.Title, "Nice3point.Revit.AddIn");
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