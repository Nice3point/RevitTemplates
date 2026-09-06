using Autodesk.Revit.Attributes;
using ModalModule.Views;
using Nice3point.Revit.Toolkit.External;

namespace RevitAddIn.Commands;

/// <summary>
///     Provides the Revit external command entry point.
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ShowModalWindowCommand : ExternalCommand
{
    public override void Execute()
    {
        var view = Host.CreateScope<ModalModuleView>();
        view.ShowDialog();
    }
}
