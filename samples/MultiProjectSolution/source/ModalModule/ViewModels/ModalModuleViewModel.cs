using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ModalModule.ViewModels;

public sealed partial class ModalModuleViewModel(ILogger<ModalModuleViewModel> logger, IOptions<JsonSerializerOptions> serializerOptions) : ObservableObject
{
    [ObservableProperty] private string? _projectName = Context.ActiveDocument?.ProjectInformation.Name;

    [RelayCommand]
    private void SaveProjectName()
    {
        var activeDocument = Context.ActiveDocument;
        if (activeDocument is null) return;

        using var transaction = new Transaction(activeDocument);
        transaction.Start("Save project name");

        activeDocument.ProjectInformation.Name = ProjectName;

        transaction.Commit();
        logger.LogInformation("Saving successful");
        logger.LogInformation("{Info}", JsonSerializer.Serialize(this, serializerOptions.Value));
    }
}