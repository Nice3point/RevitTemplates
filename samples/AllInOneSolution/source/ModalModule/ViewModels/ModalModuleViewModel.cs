using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Nice3point.Revit.Toolkit;

namespace ModalModule.ViewModels;

public sealed partial class ModalModuleViewModel(ILogger<ModalModuleViewModel> logger) : ObservableObject
{
    [ObservableProperty] private string _projectName = Context.Document.ProjectInformation.Name;

    [RelayCommand]
    private void SaveProjectName()
    {
        var transaction = new Transaction(Context.Document);
        transaction.Start("Save project name");

        Context.Document.ProjectInformation.Name = ProjectName;

        transaction.Commit();
        logger.LogInformation("Saving successful");
    }
}