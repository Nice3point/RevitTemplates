using ModalModule.Views;

namespace ModalModule.Commands;

/// <summary>
///     Command entry point invoked from the Revit AddIn Application
/// </summary>
/// <remarks>It should be registered in a DI container, and received as a service with resolved dependencies</remarks>
public sealed class ShowModalWindowService(ModalModuleView view)
{
    public void Execute()
    {
        view.ShowDialog();
    }
}