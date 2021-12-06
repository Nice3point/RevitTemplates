using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using RibbonButton = Autodesk.Revit.UI.RibbonButton;
using RibbonPanel = Autodesk.Revit.UI.RibbonPanel;

namespace Nice3point.Revit.AddIn.RevitUtils
{
    public static class RibbonUtils
    {
        /// <summary>
        ///     Adds a button to the ribbon
        /// </summary>
        public static PushButton AddPushButton(this RibbonPanel panel, Type command, string buttonText)
        {
            var pushButtonData = new PushButtonData(command.FullName, buttonText, Assembly.GetAssembly(command).Location, command.FullName);
            return (PushButton) panel.AddItem(pushButtonData);
        }

        /// <summary>
        ///     Creates a panel in the Add-ins tab
        /// </summary>
        /// <returns>New or existing Ribbon panel</returns>
        public static RibbonPanel CreatePanel(this UIControlledApplication application, string panelName)
        {
            var ribbonPanels = application.GetRibbonPanels(Tab.AddIns);
            return CreateRibbonPanel(application, panelName, ribbonPanels);
        }

        /// <summary>
        ///     Creates a panel in the specified tab
        /// </summary>
        /// <returns>New or existing Ribbon panel</returns>
        public static RibbonPanel CreatePanel(this UIControlledApplication application, string panelName, string tabName)
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
        
        /// <summary>
        ///     Adds a 16x16px-96dpi image from the URI source
        /// </summary>
        public static void SetImage(this RibbonButton button, string uri)
        {
            button.Image = new BitmapImage(new Uri(uri, UriKind.Relative));
        }
        
        /// <summary>
        ///     Adds a 32x32px-96dpi image from the URI source
        /// </summary>
        public static void SetLargeImage(this RibbonButton button, string uri)
        {
            button.LargeImage = new BitmapImage(new Uri(uri, UriKind.Relative));
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