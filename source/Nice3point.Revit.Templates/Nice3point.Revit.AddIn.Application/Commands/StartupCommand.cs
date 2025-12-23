using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;

namespace Nice3point.Revit.AddIn.Commands;

/// <summary>
///     External command entry point
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
#if (useDi && isCommandAddin)
        Host.Start();
#endif
    }
}