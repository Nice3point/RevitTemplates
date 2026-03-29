using Autodesk.Revit.Attributes;
using CommunityToolkit.Mvvm.Messaging;
using Nice3point.Revit.Extensions.UI;
using Nice3point.Revit.Toolkit.External;
using RevitAddIn.Messages;
using RevitAddIn.ViewModels;
using RevitAddIn.Views;

namespace RevitAddIn.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var focusRequest = StrongReferenceMessenger.Default.Send<FocusRequestMessage>();
        if (focusRequest is { HasReceivedResponse: true, Response: true })
        {
            return;
        }

        var viewModel = new RevitAddInViewModel();
        var view = new RevitAddInView(viewModel);
        view.Show(Application.MainWindowHandle);
    }
}