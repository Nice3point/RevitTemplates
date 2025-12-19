using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
#if (!hasUi)
using Autodesk.Revit.UI;
#endif
#if (hasUi && !useDi)
using Nice3point.Revit.AddIn.ViewModels;
#endif
#if (hasUi)
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
#if (uiModeless && useDi)
        var view = Host.GetService<Nice3point.Revit.AddInView>();
        view.Show(UiApplication.MainWindowHandle);
#elseif (uiModal && useDi)
        var view = Host.GetService<Nice3point.Revit.AddInView>();
        view.ShowDialog();
#elseif (!hasUi && useDi)
        TaskDialog.Show(Document.Title, "Nice3point.Revit.AddIn");
#elseif (uiModeless)
        var viewModel = new Nice3point.Revit.AddInViewModel();
        var view = new Nice3point.Revit.AddInView(viewModel);
        view.Show(UiApplication.MainWindowHandle);
#elseif (uiModal)
        var viewModel = new Nice3point.Revit.AddInViewModel();
        var view = new Nice3point.Revit.AddInView(viewModel);
        view.ShowDialog();
#elseif (!hasUi)
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