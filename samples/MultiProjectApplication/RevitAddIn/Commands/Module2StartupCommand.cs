using Autodesk.Revit.Attributes;
using Module2.ViewModels;
using Module2.Views;
using Nice3point.Revit.Toolkit.External;

namespace RevitAddIn.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class Module2StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new Module2ViewModel();
        var view = new Module2View(viewModel);
        view.ShowDialog();
    }
}