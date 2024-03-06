using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;

namespace Nice3point.Revit.AddIn.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
    }
}