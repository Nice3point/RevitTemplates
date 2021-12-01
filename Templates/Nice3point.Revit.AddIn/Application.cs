using System;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using Nice3point.Revit.AddIn.Commands;
using Nice3point.Revit.AddIn.RevitUtils;

namespace Nice3point.Revit.AddIn
{
    public class Application : IExternalApplication
    {
        private const string RibbonImageUri = "/ModelessAddIn;component/Resources/Icons/RibbonIcon16.png";
        private const string RibbonLargeImageUri = "/ModelessAddIn;component/Resources/Icons/RibbonIcon32.png";

        public Result OnStartup(UIControlledApplication application)
        {
            var panel = application.CreatePanel("Panel name", "Nice3point.Revit.AddIn");

            var showButton = panel.AddPushButton(typeof(Command), "Button text");
            showButton.ToolTip = "Tooltip";
            showButton.SetImage(RibbonImageUri);
            showButton.SetLargeImage(RibbonLargeImageUri);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}