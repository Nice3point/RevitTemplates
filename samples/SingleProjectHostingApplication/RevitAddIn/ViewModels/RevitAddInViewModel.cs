namespace RevitAddIn.ViewModels;

public sealed partial class RevitAddInViewModel : ObservableObject
{
    [ObservableProperty] private string _projectName = Context.ActiveDocument?.ProjectInformation.Name;

    [RelayCommand]
    private void SaveProjectName()
    {
        var activeDocument = Context.ActiveDocument;
        if (activeDocument is null) return;
        
        var transaction = new Transaction(activeDocument);
        transaction.Start("Save project name");

        activeDocument.ProjectInformation.Name = ProjectName;
        
        transaction.Commit();
    }
}