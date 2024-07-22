using ModelessModule.Views;
using ModelessModule.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace ModelessModule.Commands;

/// <summary>
///     Command entry point invoked from the Revit AddIn Application
/// </summary>
/// <remarks>It should be registered in a DI container, and received as a service with resolved dependencies</remarks>
public sealed class ShowModelessWindowService(IServiceProvider serviceProvider)
{
    public void Execute()
    {
        if (WindowController.Focus<ModelessModuleView>()) return;

        var view = serviceProvider.GetRequiredService<ModelessModuleView>();
        WindowController.Show(view, Context.UiApplication.MainWindowHandle);
    }
}