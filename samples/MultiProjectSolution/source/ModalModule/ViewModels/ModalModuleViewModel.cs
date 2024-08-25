using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ModalModule.ViewModels;

public sealed partial class ModalModuleViewModel(ILogger<ModalModuleViewModel> logger, IOptions<JsonSerializerOptions> serializerOptions) : ObservableObject
{
    [ObservableProperty] private string _projectName = Context.Document.ProjectInformation.Name;

    [RelayCommand]
    private void SaveProjectName()
    {
        using var transaction = new Transaction(Context.Document);
        transaction.Start("Save project name");

        Context.Document.ProjectInformation.Name = ProjectName;

        transaction.Commit();
        logger.LogInformation("Saving successful");
        logger.LogInformation("{Info}", JsonSerializer.Serialize(this, serializerOptions.Value));
    }
}