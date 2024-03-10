using Nice3point.Revit.Toolkit.External;
using ModelessModule.Views;
using ModelessModule.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace ModelessModule.CommandHandlers;

/// <summary>
///     Command entry point invoked from the Revit AddIn Application
/// </summary>
public class ShowWindowHandler(IServiceProvider serviceProvider)
{
    public void Execute(ExternalCommand shell)
    {
        if (WindowController.Focus<ModelessModuleView>()) return;

        var view = serviceProvider.GetService<ModelessModuleView>();
        WindowController.Show(view, shell.UiApplication.MainWindowHandle);
    }
}