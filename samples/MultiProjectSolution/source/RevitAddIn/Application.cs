using Nice3point.Revit.Toolkit.External;
using RevitAddIn.Commands;

namespace RevitAddIn;

/// <summary>
///     Application entry point
/// </summary>
[UsedImplicitly]
public class Application : ExternalApplication
{
    public override void OnStartup()
    {
        Host.Start();
        CreateRibbon();
    }

    public override void OnShutdown()
    {
        Host.Stop();
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