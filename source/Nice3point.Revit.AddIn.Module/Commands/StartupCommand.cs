using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
#if (NoWindow)
using Autodesk.Revit.UI;
#endif
#if (!NoWindow)
using Nice3point.Revit.AddIn.ViewModels;
#endif
#if (!NoWindow)
using Nice3point.Revit.AddIn.Views;
#endif
#if (ModelessWindow)
using Nice3point.Revit.AddIn.Utils;
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
#if (ModelessWindow)
        if (WindowController.Focus<Nice3point.Revit.AddInView>()) return;

        var viewModel = new Nice3point.Revit.AddInViewModel();
        var view = new Nice3point.Revit.AddInView(viewModel);
        WindowController.Show(view, UiApplication.MainWindowHandle);
#elseif (ModalWindow)
        var viewModel = new Nice3point.Revit.AddInViewModel();
        var view = new Nice3point.Revit.AddInView(viewModel);
        view.ShowDialog();
#elseif (NoWindow)
        TaskDialog.Show(Document.Title, "Nice3point.Revit.AddIn");
#endif
    }
}