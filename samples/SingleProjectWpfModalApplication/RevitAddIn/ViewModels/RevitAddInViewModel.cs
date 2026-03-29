namespace RevitAddIn.ViewModels;

public sealed partial class RevitAddInViewModel : ObservableObject
{
    public RevitAddInViewModel()
    {
        ProjectName = RevitContext.ActiveDocument?.ProjectInformation.Name;
    }
    
    [ObservableProperty]
    public partial string? ProjectName { get; set; }

    [RelayCommand]
    private void SaveProjectName()
    {
        var activeDocument = RevitContext.ActiveDocument;
        if (activeDocument is null) return;
        
        var transaction = new Transaction(activeDocument!);
        transaction.Start("Save project name");

        activeDocument.ProjectInformation.Name = ProjectName;
        
        transaction.Commit();
    }
}