using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Microsoft.Extensions.Logging;
using ModelessModule.Services;
using Nice3point.Revit.Toolkit.External.Handlers;
using Nice3point.Revit.Toolkit.Options;

namespace ModelessModule.ViewModels;

public sealed partial class ModelessModuleViewModel(ModelessController modelessController, ILogger<ModelessModuleViewModel> logger) : ObservableObject
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
            
            logger.LogInformation("Selection successful");
        });
    }

    [RelayCommand]
    private async Task SelectDelayedElementAsync()
    {
        Status = "Wait 2 second...";
        await Task.Delay(TimeSpan.FromSeconds(2));

        await _asyncExternalHandler.RaiseAsync(application =>
        {
            var selectionConfiguration = new SelectionConfiguration();
            modelessController.Hide();

            var reference = application.ActiveUIDocument.Selection.PickObject(ObjectType.Element, selectionConfiguration.Filter);
            var element = reference.ElementId.ToElement(application.ActiveUIDocument.Document)!;
            modelessController.Show();

            Element = element.Name;
            Category = element.Category.Name;
            
            logger.LogInformation("Selection successful");
        });

        Status = string.Empty;
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
        logger.LogInformation("Deletion successful");
    }
}