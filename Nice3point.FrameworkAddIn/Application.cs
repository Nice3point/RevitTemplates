using System;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using Nice3point.FrameworkAddIn.Commands;
using RibbonPanel = Autodesk.Revit.UI.RibbonPanel;

namespace Nice3point.FrameworkAddIn
{
    public class Application : IExternalApplication
    {
        private const string ButtonImageUrl = "pack://application:,,,/Nice3point.FrameworkAddIn;component/Resources/Icons/RibbonIcon16.png";
        private const string ButtonLargeImageUrl = "pack://application:,,,/Nice3point.FrameworkAddIn;component/Resources/Icons/RibbonIcon32.png";

        public Result OnStartup(UIControlledApplication application)
        {
            var panel = CreateRibbonTab(application, "Panel name", "Tab name");
            if (panel.AddItem(
                    new PushButtonData(nameof(Command),
                        "Button text",
                        Assembly.GetExecutingAssembly().Location,
                        typeof(Command).FullName))
                is PushButton showPanel)
            {
                showPanel.ToolTip    = "Tooltip";
                showPanel.Image      = new BitmapImage(new Uri(ButtonImageUrl));
                showPanel.LargeImage = new BitmapImage(new Uri(ButtonLargeImageUrl));
            }

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        private static RibbonPanel CreateRibbonTab(UIControlledApplication application, string panelName, string tabName = "")
        {
            if (string.IsNullOrEmpty(tabName)) return application.CreateRibbonPanel(panelName);
            var ribbonTab = ComponentManager.Ribbon.Tabs.FirstOrDefault(tab => tab.Id.Equals(tabName));
            if (ribbonTab == null) application.CreateRibbonTab(tabName);
            return application.CreateRibbonPanel(tabName, panelName);
        }
    }
}