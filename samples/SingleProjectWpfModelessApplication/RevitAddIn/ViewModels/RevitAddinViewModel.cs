using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CommunityToolkit.Mvvm.Messaging;
using Nice3point.Revit.Toolkit.External;
using Nice3point.Revit.Toolkit.Options;
using RevitAddIn.Messages;

namespace RevitAddIn.ViewModels;

public sealed partial class RevitAddInViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Element { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string Category { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string Status { get; private set; } = string.Empty;

    [RelayCommand]
    private void ShowSummary()
    {
        ShowSummaryEvent.Raise();
    }

    [RelayCommand]
    private async Task DeleteElementAsync()
    {
        var deletedId = await DeleteElementAsyncEvent.RaiseAsync();

        TaskDialog.Show("Deleted element", $"ID: {deletedId}");
    }

    [RelayCommand]
    private async Task SelectDelayedElementAsync()
    {
        Status = "Wait 2 second...";
        await Task.Delay(TimeSpan.FromSeconds(2));

        await SelectDelayedElementAsyncEvent.RaiseAsync();

        Status = string.Empty;
    }

    [ExternalEvent]
    private void ShowSummary(UIApplication application)
    {
        var selectionConfiguration = new SelectionConfiguration();
        var reference = application.ActiveUIDocument.Selection.PickObject(ObjectType.Element, selectionConfiguration.Filter);
        var element = reference.ElementId.ToElement(application.ActiveUIDocument.Document)!;

        Element = element.Name;
        Category = element.Category.Name;
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
        StrongReferenceMessenger.Default.Send<HideRequestMessage>();

        var reference = application.ActiveUIDocument.Selection.PickObject(ObjectType.Element, selectionConfiguration.Filter);
        var element = reference.ElementId.ToElement(application.ActiveUIDocument.Document)!;
        StrongReferenceMessenger.Default.Send<ShowRequestMessage>();

        Element = element.Name;
        Category = element.Category.Name;
    }
}