using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
#if (NoWindow)
using Autodesk.Revit.UI;
#endif
#if (!NoWindow && !UseIoc)
using Nice3point.Revit.AddIn.ViewModels;
#endif
#if (!NoWindow)
using Nice3point.Revit.AddIn.Views;
#endif
#if (log && Command && !UseIoc)
using Serilog;
using Serilog.Events;
#endif

namespace Nice3point.Revit.AddIn.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
#if (log && Command && !UseIoc)
        var logger = CreateLogger();
#endif
#if (Modeless && UseIoc)
        var view = Host.GetService<Nice3point.Revit.AddInView>();
        view.Show(UiApplication.MainWindowHandle);
#elseif (Modal && UseIoc)
        var view = Host.GetService<Nice3point.Revit.AddInView>();
        view.ShowDialog();
#elseif (NoWindow && UseIoc)
        TaskDialog.Show(Document.Title, "Nice3point.Revit.AddIn");
#elseif (Modeless)
        var viewModel = new Nice3point.Revit.AddInViewModel();
        var view = new Nice3point.Revit.AddInView(viewModel);
        view.Show(UiApplication.MainWindowHandle);
#elseif (Modal)
        var viewModel = new Nice3point.Revit.AddInViewModel();
        var view = new Nice3point.Revit.AddInView(viewModel);
        view.ShowDialog();
#elseif (NoWindow)
        TaskDialog.Show(Document.Title, "Nice3point.Revit.AddIn");
#endif
    }
#if (log && Command && !UseIoc)

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