namespace Module2.ViewModels;

public sealed partial class Module2ViewModel : ObservableObject
{
    [ObservableProperty] private string _projectName = Context.Document.ProjectInformation.Name;

    [RelayCommand]
    private void SaveProjectName()
    {
        var transaction = new Transaction(Context.Document);
        transaction.Start("Save project name");

        Context.Document.ProjectInformation.Name = ProjectName;
        
        transaction.Commit();
    }
}