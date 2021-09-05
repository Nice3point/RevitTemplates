using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using RibbonPanel = Autodesk.Revit.UI.RibbonPanel;

namespace Nice3point.Revit.AddIn.RevitUtils
{
    public static class RibbonUtils
    {
        public static PushButton AddPushButton(this RibbonPanel panel, Type command, string commandName, string buttonText)
        {
            return (PushButton)panel.AddItem(
                new PushButtonData(commandName,
                    buttonText,
                    Assembly.GetExecutingAssembly().Location,
                    command.FullName));
        }
        
        public static RibbonPanel CreateRibbonPanel(UIControlledApplication application, string panelName)
        {
            var ribbonPanels = application.GetRibbonPanels(Tab.AddIns);
            return CreateRibbonPanel(application, panelName, ribbonPanels);
        }

        public static RibbonPanel CreateRibbonPanel(UIControlledApplication application, string panelName, string tabName)
        {
            var ribbonTab = ComponentManager.Ribbon.Tabs.FirstOrDefault(tab => tab.Id.Equals(tabName));
            if (ribbonTab is null)
            {
                application.CreateRibbonTab(tabName);
                return application.CreateRibbonPanel(tabName, panelName);
            }

            var ribbonPanels = application.GetRibbonPanels(tabName);
            return CreateRibbonPanel(application, panelName, ribbonPanels);
        }

        private static RibbonPanel CreateRibbonPanel(UIControlledApplication application, string panelName, IEnumerable<RibbonPanel> ribbonPanels)
        {
            var ribbonPanel = ribbonPanels.FirstOrDefault(panel => panel.Name.Equals(panelName));
            return ribbonPanel ?? application.CreateRibbonPanel(panelName);
        }
    }
}