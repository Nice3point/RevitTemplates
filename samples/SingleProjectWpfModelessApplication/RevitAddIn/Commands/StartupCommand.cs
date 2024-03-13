using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using RevitAddIn.ViewModels;
using RevitAddIn.Views;
using RevitAddIn.Utils;

namespace RevitAddIn.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        if (WindowController.Focus<RevitAddInView>()) return;

        var viewModel = new RevitAddInViewModel();
        var view = new RevitAddInView(viewModel);
        WindowController.Show(view, UiApplication.MainWindowHandle);
    }
}