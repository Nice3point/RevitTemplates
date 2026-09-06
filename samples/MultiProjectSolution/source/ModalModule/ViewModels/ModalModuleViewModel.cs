using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RevitAddIn.ServiceDefaults.Serialization;

namespace ModalModule.ViewModels;

/// <summary>
///     Represents the data and commands for the add-in window.
/// </summary>
/// <param name="logger">The logger for window operations.</param>
/// <param name="serializerOptions">The JSON serialization options for project metadata.</param>
public sealed partial class ModalModuleViewModel(ILogger<ModalModuleViewModel> logger, IOptions<JsonSerializerOptions> serializerOptions) : ObservableObject
{
    [ObservableProperty]
    public partial string? ProjectName { get; set; } = RevitContext.ActiveDocument?.ProjectInformation.Name;

    [RelayCommand]
    private void SaveProjectName()
    {
        var activeDocument = RevitContext.ActiveDocument;
        if (activeDocument is null) return;

        using var transaction = new Transaction(activeDocument);
        transaction.Start("Save project name");

        activeDocument.ProjectInformation.Name = ProjectName;

        transaction.Commit();
        logger.LogInformation("Saving successful");

        var snapshot = new ProjectMetadata(ProjectName);
        logger.LogInformation("{Info}", JsonSerializer.Serialize(snapshot, serializerOptions.Value));
    }
}
