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

        var showButton = panel.AddPushButton<StartupCommand>("Execute");
        showButton.SetImage("/RevitAddIn;component/Resources/Icons/RibbonIcon16.png");
        showButton.SetLargeImage("/RevitAddIn;component/Resources/Icons/RibbonIcon32.png");
    }
}