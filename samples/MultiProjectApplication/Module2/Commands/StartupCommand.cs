using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using Module2.ViewModels;
using Module2.Views;

namespace Module2.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new Module2ViewModel();
        var view = new Module2View(viewModel);
        view.ShowDialog();
    }
}