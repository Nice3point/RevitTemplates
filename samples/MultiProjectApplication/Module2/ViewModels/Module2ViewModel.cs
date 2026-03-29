namespace Module2.ViewModels;

public sealed partial class Module2ViewModel : ObservableObject
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