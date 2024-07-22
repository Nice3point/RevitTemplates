using Autodesk.Revit.Attributes;
using ModelessModule.Commands;
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
        Host.GetService<ShowModelessWindowService>().Execute();
    }
}