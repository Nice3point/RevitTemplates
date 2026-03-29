using Autodesk.Revit.Attributes;
using CommunityToolkit.Mvvm.Messaging;
using ModelessModule.Messages;
using ModelessModule.Views;
using Nice3point.Revit.Extensions.UI;
using Nice3point.Revit.Toolkit.External;

namespace RevitAddIn.Commands;

/// <summary>
///     External command entry point
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ShowModelessWindowCommand : ExternalCommand
{
    public override void Execute()
    {
        var messenger = Host.GetService<IMessenger>();
        var focusRequest = messenger.Send<FocusRequestMessage>();
        if (focusRequest is { HasReceivedResponse: true, Response: true })
        {
            return;
        }

        var view = Host.CreateScope<ModelessModuleView>();
        view.Show(Application.MainWindowHandle);
    }
}