using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
#if (!useUi)
using Autodesk.Revit.UI;
#endif
#if (useUi && !useDi)
using Nice3point.Revit.AddIn._1.ViewModels;
#endif
#if (useUi)
using Nice3point.Revit.AddIn._1.Views;
#endif

namespace Nice3point.Revit.AddIn._1.Commands;

/// <summary>
///     External command entry point.
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
#if (diHosting && isCommandAddin)
public class StartupCommand : AsyncExternalCommand
#else
public class StartupCommand : ExternalCommand
#endif
{
#if (diHosting && isCommandAddin)
    public override async Task ExecuteAsync()
#else
    public override void Execute()
#endif
    {
#if (isCommandAddin && diHosting)
        await Host.StartAsync();
#elseif (isCommandAddin && useDi)
        Host.Start();
#endif
#if (useUi && useDi)
        var view = Host.GetService<Nice3point_Revit_AddIn__1View>();
        view.ShowDialog();
#elseif (!useUi && useDi)
        TaskDialog.Show(Application.ActiveUIDocument.Document.Title, "Nice3point.Revit.AddIn.1");
#elseif (useUi)
        var viewModel = new Nice3point_Revit_AddIn__1ViewModel();
        var view = new Nice3point_Revit_AddIn__1View(viewModel);
        view.ShowDialog();
#elseif (!useUi)
        TaskDialog.Show(Application.ActiveUIDocument.Document.Title, "Nice3point.Revit.AddIn.1");
#endif
#if (isCommandAddin && diHosting)
        await Host.StopAsync();
#endif
    }
}
