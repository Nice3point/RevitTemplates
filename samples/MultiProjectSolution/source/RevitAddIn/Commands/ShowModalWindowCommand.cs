using Autodesk.Revit.Attributes;
using ModalModule.Views;
using Nice3point.Revit.Toolkit.External;

namespace RevitAddIn.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ShowModalWindowCommand : ExternalCommand
{
    public override void Execute()
    {
        var view = Host.GetService<ModalModuleView>();
        view.ShowDialog();
    }
}