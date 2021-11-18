using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Nice3point.Revit.AddIn.ViewModels;
using Nice3point.Revit.AddIn.Views;

namespace Nice3point.Revit.AddIn.Commands
{
    [Transaction(TransactionMode.ReadOnly)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiDocument = commandData.Application.ActiveUIDocument;
            var document = uiDocument.Document;
#if (CommandStyle)
            /*caret*/
#else

#endif
            var viewModel = new SimpleViewModel();
            var view = new SimpleView(viewModel);
            view.ShowDialog();

            return Result.Succeeded;
        }
    }
}