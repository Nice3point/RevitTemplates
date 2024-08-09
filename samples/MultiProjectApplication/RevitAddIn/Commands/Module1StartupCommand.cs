using Autodesk.Revit.Attributes;
using Module1.ViewModels;
using Module1.Views;
using Nice3point.Revit.Toolkit.External;

namespace RevitAddIn.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class Module1StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new Module1ViewModel();
        var view = new Module1View(viewModel);
        view.ShowDialog();
    }
}