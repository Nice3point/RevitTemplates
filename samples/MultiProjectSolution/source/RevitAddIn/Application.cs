using Nice3point.Revit.Extensions.UI;
using Nice3point.Revit.Toolkit.External;
using RevitAddIn.Commands;

namespace RevitAddIn;

/// <summary>
///     Application entry point
/// </summary>
[UsedImplicitly]
public class Application : AsyncExternalApplication
{
    public override async Task OnStartupAsync()
    {
        await Host.StartAsync();
        CreateRibbon();
    }

    public override async Task OnShutdownAsync()
    {
        await Host.StopAsync();
    }

    private void CreateRibbon()
    {
        var panel = Application.CreatePanel("Commands", "RevitAddIn");

        panel.AddPushButton<ShowModalWindowCommand>("Show\nModal window")
            .SetImage("/RevitAddIn;component/Resources/Icons/RibbonIcon16.png")
            .SetLargeImage("/RevitAddIn;component/Resources/Icons/RibbonIcon32.png");

        panel.AddPushButton<ShowModelessWindowCommand>("Show\nModeless window")
            .SetImage("/RevitAddIn;component/Resources/Icons/RibbonIcon16.png")
            .SetLargeImage("/RevitAddIn;component/Resources/Icons/RibbonIcon32.png");
    }
}