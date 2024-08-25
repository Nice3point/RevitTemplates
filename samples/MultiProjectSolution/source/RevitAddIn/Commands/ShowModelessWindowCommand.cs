using Autodesk.Revit.Attributes;
using ModelessModule.Services;
using ModelessModule.Views;
using Nice3point.Revit.Toolkit.External;

namespace RevitAddIn.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ShowModelessWindowCommand : ExternalCommand
{
    public override void Execute()
    {
        var modelessController = Host.GetService<ModelessController>();
        var view = Host.GetService<ModelessModuleView>();
        modelessController.Show(view);
    }
}