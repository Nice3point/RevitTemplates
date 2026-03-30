using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;

namespace Nice3point.Revit.AddIn._1.Commands;

/// <summary>
///     External command entry point
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
#if (diHosting && isCommandAddin)
public class StartupCommand : AsyncExternalCommand
{
    public override async Task ExecuteAsync()
    {
        await Host.StartAsync();
    }
}
#else
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
#if (useDi && isCommandAddin)
        Host.Start();
#endif
    }
}
#endif