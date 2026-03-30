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
#else
public class StartupCommand : ExternalCommand
#endif
{
#if (diHosting && isCommandAddin)
    public override async Task ExecuteAsync()
#else
    public override void Execute()
#endif
    {
#if (isCommandAddin && diHosting)
        await Host.StartAsync();
        await Host.StopAsync();
#elseif (isCommandAddin && useDi)
        Host.Start();
#endif
    }
}