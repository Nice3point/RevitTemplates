using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
#if (NoWindow)
using Autodesk.Revit.UI;
#endif
#if (!NoWindow)
using Nice3point.Revit.AddIn.ViewModels;
using Nice3point.Revit.AddIn.Views;
#endif
#if (ModelessWindow)
using Nice3point.Revit.AddIn.Utils;
#endif
#if (Logger && CommandStyle)
using Serilog.Events;
#endif

namespace Nice3point.Revit.AddIn.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class Command : ExternalCommand
{
    public override void Execute()
    {
#if (Logger && CommandStyle)
        CreateLogger();
#endif
#if (ModelessWindow)
        if (WindowController.Focus<Nice3point.Revit.AddInView>()) return;

        var viewModel = new Nice3point.Revit.AddInViewModel();
        var view = new Nice3point.Revit.AddInView(viewModel);
        WindowController.Show(view, UiApplication.MainWindowHandle);
#elif (ModalWindow)
        var viewModel = new Nice3point.Revit.AddInViewModel();
        var view = new Nice3point.Revit.AddInView(viewModel);
        view.ShowDialog();
#elif (NoWindow)
        TaskDialog.Show(Document.Title, "Nice3point.Revit.AddIn");
#endif
#if (Logger && CommandStyle)
        Log.CloseAndFlush();
#endif
    }
#if (Logger && CommandStyle)

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
#endif
}