using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Nice3point.Revit.Toolkit.External.Handlers;
using Nice3point.Revit.Toolkit.Options;
using RevitAddIn.Utils;
using RevitAddIn.Views;

namespace RevitAddIn.ViewModels;

public sealed partial class RevitAddInViewModel : ObservableObject
{
    private readonly ActionEventHandler _externalHandler = new();
    private readonly AsyncEventHandler _asyncExternalHandler = new();
    private readonly AsyncEventHandler<ElementId> _asyncIdExternalHandler = new();

    [ObservableProperty] private string _element;
    [ObservableProperty] private string _category;
    [ObservableProperty] private string _status;

    [RelayCommand]
    private void ShowSummary()
    {
        _externalHandler.Raise(application =>
        {
            var selectionConfiguration = new SelectionConfiguration();
            var reference = application.ActiveUIDocument.Selection.PickObject(ObjectType.Element, selectionConfiguration.Filter);
            var element = reference.ElementId.ToElement(application.ActiveUIDocument.Document)!;

            Element = element.Name;
            Category = element.Category.Name;
        });
    }

    [RelayCommand]
    private async Task DeleteElementAsync()
    {
        var deletedId = await _asyncIdExternalHandler.RaiseAsync(application =>
        {
            var document = application.ActiveUIDocument.Document;

            var selectionConfiguration = new SelectionConfiguration();
            var reference = application.ActiveUIDocument.Selection.PickObject(ObjectType.Element, selectionConfiguration.Filter);

            var transaction = new Transaction(document);
            transaction.Start("Delete element");
            document.Delete(reference.ElementId);
            transaction.Commit();

            return reference.ElementId;
        });

        TaskDialog.Show("Deleted element", $"ID: {deletedId}");
    }

    [RelayCommand]
    private async Task SelectDelayedElementAsync()
    {
        Status = "Wait 2 second...";
        await Task.Delay(TimeSpan.FromSeconds(2));

        await _asyncExternalHandler.RaiseAsync(application =>
        {
            var selectionConfiguration = new SelectionConfiguration();
            WindowController.Hide<RevitAddInView>();

            var reference = application.ActiveUIDocument.Selection.PickObject(ObjectType.Element, selectionConfiguration.Filter);
            var element = reference.ElementId.ToElement(application.ActiveUIDocument.Document)!;
            WindowController.Show<RevitAddInView>();

            Element = element.Name;
            Category = element.Category.Name;
        });

        Status = string.Empty;
    }
}