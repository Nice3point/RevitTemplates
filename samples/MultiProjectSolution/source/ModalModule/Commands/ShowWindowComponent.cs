using ModalModule.Views;

namespace ModalModule.Commands;

/// <summary>
///     Command entry point invoked from the Revit AddIn Application
/// </summary>
public class ShowWindowComponent(ModalModuleView view)
{
    public void Execute()
    {
        view.ShowDialog();
    }
}