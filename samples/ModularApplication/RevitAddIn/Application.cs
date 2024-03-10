using Nice3point.Revit.Toolkit.External;

namespace RevitAddIn;

/// <summary>
///     Application entry point
/// </summary>
[UsedImplicitly]
public class Application : ExternalApplication
{
    public override void OnStartup()
    {
        CreateRibbon();
    }

    private void CreateRibbon()
    {
        var panel = Application.CreatePanel("Commands", "RevitAddIn");

        var showButton = panel.AddPushButton<Module1.Commands.StartupCommand>("Execute");
        showButton.SetImage("/RevitAddIn;component/Resources/Icons/RibbonIcon16.png");
        showButton.SetLargeImage("/RevitAddIn;component/Resources/Icons/RibbonIcon32.png");

        var showButton2 = panel.AddPushButton<Module2.Commands.StartupCommand>("Execute");
        showButton2.SetImage("/RevitAddIn;component/Resources/Icons/RibbonIcon16.png");
        showButton2.SetLargeImage("/RevitAddIn;component/Resources/Icons/RibbonIcon32.png");
    }
}