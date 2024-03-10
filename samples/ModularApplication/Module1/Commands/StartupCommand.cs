using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using Module1.ViewModels;
using Module1.Views;

namespace Module1.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new Module1ViewModel();
        var view = new Module1View(viewModel);
        view.ShowDialog();
    }
}