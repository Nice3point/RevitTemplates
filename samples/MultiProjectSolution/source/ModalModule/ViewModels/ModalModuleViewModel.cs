using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nice3point.Revit.Toolkit;

namespace ModalModule.ViewModels;

public sealed partial class ModalModuleViewModel(ILogger<ModalModuleViewModel> logger, IOptions<JsonSerializerOptions> serializerOptions) : ObservableObject
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
        logger.LogInformation("{Info}", JsonSerializer.Serialize(this, serializerOptions.Value));
    }
}