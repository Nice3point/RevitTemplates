using Autodesk.Revit.Attributes;
using ModelessModule.CommandHandlers;
using Nice3point.Revit.Toolkit.External;

namespace RevitAddIn.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ShowModelessWindowCommand : ExternalCommand
{
    public override void Execute()
    {
        Host.GetService<ShowWindowHandler>().Execute(this);
    }
}