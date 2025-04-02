namespace RevitAddIn.ViewModels;

public sealed partial class RevitAddInViewModel : ObservableObject
{
    [ObservableProperty] private string? _projectName = string.Empty;

    public RevitAddInViewModel()
    {
        ProjectName = Context.ActiveDocument?.ProjectInformation.Name;
    }

    [RelayCommand]
    private void SaveProjectName()
    {
        var activeDocument = Context.ActiveDocument;
        if (activeDocument is null) return;
        
        var transaction = new Transaction(activeDocument!);
        transaction.Start("Save project name");

        activeDocument.ProjectInformation.Name = ProjectName;
        
        transaction.Commit();
    }
}