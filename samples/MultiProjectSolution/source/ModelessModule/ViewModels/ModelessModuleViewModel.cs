using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using ModelessModule.Messages;
using ModelessModule.Models;
using ModelessModule.Services;
using Nice3point.Revit.Toolkit.External;
using Nice3point.Revit.Toolkit.Options;

namespace ModelessModule.ViewModels;

public sealed partial class ModelessModuleViewModel(ElementMetadataExtractionService elementService, IMessenger messenger, ILogger<ModelessModuleViewModel> logger) : ObservableObject
{
    [ObservableProperty]
    public partial ElementMetadata? ElementMetadata { get; set; }

    [ObservableProperty]
    public partial string Status { get; private set; } = string.Empty;

    [RelayCommand]
    private void ShowSummary()
    {
        ShowSummaryEvent.Raise();
        
        logger.LogInformation("Selection successful");
    }

    [RelayCommand]
    private async Task DeleteElementAsync()
    {
        var deletedId = await DeleteElementAsyncEvent.RaiseAsync();

        logger.LogInformation("Deletion successful");
        TaskDialog.Show("Deleted element", $"ID: {deletedId}");
    }

    [RelayCommand]
    private async Task SelectDelayedElementAsync()
    {
        Status = "Wait 2 second...";
        await Task.Delay(TimeSpan.FromSeconds(2));

        await SelectDelayedElementAsyncEvent.RaiseAsync();

        logger.LogInformation("Selection successful");
        Status = string.Empty;
    }
    
    [ExternalEvent]
    private void ShowSummary(UIApplication application)
    {
        var selectionConfiguration = new SelectionConfiguration();
        var reference = application.ActiveUIDocument.Selection.PickObject(ObjectType.Element, selectionConfiguration.Filter);
        var element = reference.ElementId.ToElement(application.ActiveUIDocument.Document)!;

        ElementMetadata = elementService.ExtractMetadata(element);
    }
    
    [ExternalEvent]
    private ElementId DeleteElement(UIApplication application)
    {
        var document = application.ActiveUIDocument.Document;

        var selectionConfiguration = new SelectionConfiguration();
        var reference = application.ActiveUIDocument.Selection.PickObject(ObjectType.Element, selectionConfiguration.Filter);

        var transaction = new Transaction(document);
        transaction.Start("Delete element");
        document.Delete(reference.ElementId);
        transaction.Commit();

        return reference.ElementId;
    }
    
    
    [ExternalEvent]
    private void SelectDelayedElement(UIApplication application)
    {
        var selectionConfiguration = new SelectionConfiguration();
        messenger.Send<HideRequestMessage>();

        var reference = application.ActiveUIDocument.Selection.PickObject(ObjectType.Element, selectionConfiguration.Filter);
        var element = reference.ElementId.ToElement(application.ActiveUIDocument.Document)!;
        messenger.Send<ShowRequestMessage>();

        ElementMetadata = elementService.ExtractMetadata(element);
    }
}