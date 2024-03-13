using Autodesk.Revit.Attributes;
using ModalModule.CommandHandlers;
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
        Host.GetService<ShowWindowHandler>().Execute(this);
    }
}