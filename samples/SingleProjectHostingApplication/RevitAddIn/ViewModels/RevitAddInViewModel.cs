namespace RevitAddIn.ViewModels;

/// <summary>
///     Represents the data and commands for the add-in window.
/// </summary>
public sealed partial class RevitAddInViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string? ProjectName { get; set; } = RevitContext.ActiveDocument?.ProjectInformation.Name;

    [RelayCommand]
    private void SaveProjectName()
    {
        var activeDocument = RevitContext.ActiveDocument;
        if (activeDocument is null) return;

        var transaction = new Transaction(activeDocument);
        transaction.Start("Save project name");

        activeDocument.ProjectInformation.Name = ProjectName;

        transaction.Commit();
    }
}
