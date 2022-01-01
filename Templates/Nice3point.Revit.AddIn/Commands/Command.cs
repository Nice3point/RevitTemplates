using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#if (ModelessWindow)
using Nice3point.Revit.AddIn.Commands.Handlers;
using Nice3point.Revit.AddIn.Core;
#endif
#if (!NoWindow)
using Nice3point.Revit.AddIn.ViewModels;
using Nice3point.Revit.AddIn.Views;
#endif

namespace Nice3point.Revit.AddIn.Commands;

#if (CommandStyle)
[UsedImplicitly]
#endif
[Transaction(TransactionMode.Manual)]
public class Command : IExternalCommand
{
#if (ModelessWindow)
    private static Nice3point.Revit.AddInView _view;
    public static readonly CommandEventHandler EventHandler = new();

#endif
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
#if (ModelessWindow)
        if (_view is not null && _view.IsLoaded)
        {
            _view.Focus();
            return Result.Succeeded;
        }

        RevitApi.UiApplication = commandData.Application;
        var viewModel = new Nice3point.Revit.AddInViewModel();
        _view = new Nice3point.Revit.AddInView(viewModel);
        _view.Show();
#elif (ModalWindow)
        var uiDocument = commandData.Application.ActiveUIDocument;
        var document = uiDocument.Document;

        var viewModel = new Nice3point.Revit.AddInViewModel();
        var view = new Nice3point.Revit.AddInView(viewModel);
        view.ShowDialog();
#elif (NoWindow)
        var uiDocument = commandData.Application.ActiveUIDocument;
        var document = uiDocument.Document;

        TaskDialog.Show("Revit add-in", "Nice3point.Revit.AddIn");
#endif

        return Result.Succeeded;
    }
}