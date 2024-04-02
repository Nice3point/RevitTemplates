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
        CreateRibbon();
    }

    private void CreateRibbon()
    {
        var panel = Application.CreatePanel("Commands", "RevitAddIn");

        panel.AddPushButton<StartupCommand>("Execute")
            .SetImage("/RevitAddIn;component/Resources/Icons/RibbonIcon16.png")
            .SetLargeImage("/RevitAddIn;component/Resources/Icons/RibbonIcon32.png");
    }
}