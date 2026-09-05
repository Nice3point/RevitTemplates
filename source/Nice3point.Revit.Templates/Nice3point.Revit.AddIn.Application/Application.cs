using Nice3point.Revit.Toolkit.External;
#if (isApplicationAddin)
using Nice3point.Revit.AddIn._1.Commands;
#endif

namespace Nice3point.Revit.AddIn._1;

/// <summary>
///     Application entry point
/// </summary>
[UsedImplicitly]
#if (diHosting && isApplicationAddin)
public class Application : AsyncExternalApplication
#elseif (isApplicationAddin)
public class Application : ExternalApplication
#else
public class Application : ExternalDBApplication
#endif
{
#if (diHosting && isApplicationAddin)
    public override async Task OnStartupAsync()
#else
    public override void OnStartup()
#endif
    {
#if (diHosting && isApplicationAddin)
        await Host.StartAsync();
#elseif (useDi)
        Host.Start();
#endif
#if (isApplicationAddin)
        CreateRibbon();
#endif
    }
#if (diHosting)

#if (isApplicationAddin)
    public override async Task OnShutdownAsync()
#else
    public override void OnShutdown()
#endif
    {
#if (isApplicationAddin)
        await Host.StopAsync();
#else
        Host.Stop();
#endif
    }
#endif
#if (isApplicationAddin)

    private void CreateRibbon()
    {
        var panel = Application.CreatePanel("Commands", "Nice3point.Revit.AddIn.1");

        panel.AddPushButton<StartupCommand>("Execute")
            .SetImage("/Nice3point.Revit.AddIn.1;component/Resources/Icons/RibbonIcon16.png")
            .SetLargeImage("/Nice3point.Revit.AddIn.1;component/Resources/Icons/RibbonIcon32.png");
    }
#endif
}
