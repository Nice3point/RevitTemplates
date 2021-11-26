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
        /// <summary>
        ///     Adds a button to the ribbon
        /// </summary>
        public static PushButton AddPushButton(this RibbonPanel panel, Type command, string commandName, string buttonText)
        {
            var pushButtonData = new PushButtonData(commandName, buttonText, Assembly.GetAssembly(command).Location, command.FullName);
            return (PushButton) panel.AddItem(pushButtonData);
        }

        /// <summary>
        ///     Creates a panel in the Add-ins tab.
        /// </summary>
        public static RibbonPanel CreateRibbonPanel(UIControlledApplication application, string panelName)
        {
            var ribbonPanels = application.GetRibbonPanels(Tab.AddIns);
            return CreateRibbonPanel(application, panelName, ribbonPanels);
        }

        /// <summary>
        ///     Creates a panel in a new tab.
        /// </summary>
        public static RibbonPanel CreateRibbonPanel(UIControlledApplication application, string tabName, string panelName)
        {
            var ribbonTab = ComponentManager.Ribbon.Tabs.FirstOrDefault(tab => tab.Id.Equals(tabName));
            if (ribbonTab is null)
            {
                application.CreateRibbonTab(tabName);
                return application.CreateRibbonPanel(tabName, panelName);
            }

            var ribbonPanels = application.GetRibbonPanels(tabName);
            return CreateRibbonPanel(application, tabName, panelName, ribbonPanels);
        }

        private static RibbonPanel CreateRibbonPanel(UIControlledApplication application, string panelName, IEnumerable<RibbonPanel> ribbonPanels)
        {
            var ribbonPanel = ribbonPanels.FirstOrDefault(panel => panel.Name.Equals(panelName));
            return ribbonPanel ?? application.CreateRibbonPanel(panelName);
        }

        private static RibbonPanel CreateRibbonPanel(UIControlledApplication application, string tabName, string panelName, IEnumerable<RibbonPanel> ribbonPanels)
        {
            var ribbonPanel = ribbonPanels.FirstOrDefault(panel => panel.Name.Equals(panelName));
            return ribbonPanel ?? application.CreateRibbonPanel(tabName, panelName);
        }
    }
}