using Nice3point.Revit.Toolkit.External;
using ModalModule.Views;

namespace ModalModule.CommandHandlers;

/// <summary>
///     Command entry point invoked from the Revit AddIn Application
/// </summary>
public class ShowWindowHandler(ModalModuleView view)
{
    public void Execute(ExternalCommand shell)
    {
        view.ShowDialog();
    }
}