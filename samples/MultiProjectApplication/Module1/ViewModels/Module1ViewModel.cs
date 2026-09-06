using Module3;

namespace Module1.ViewModels;

/// <summary>
///     Represents the data and commands for the add-in window.
/// </summary>
public sealed partial class Module1ViewModel : ObservableObject
{
    private readonly Document _document = RevitContext.ActiveDocument!;
    private readonly ProjectMetadataContext _context;

    public Module1ViewModel()
    {
        _context = new ProjectMetadataContext(_document);
        ProjectName = _context.Load().Name;
    }

    [ObservableProperty]
    public partial string ProjectName { get; set; }

    [RelayCommand]
    private void SaveProjectName()
    {
        var data = _context.Load();
        data.Name = ProjectName;

        using var transaction = new Transaction(_document, "Save project data");
        transaction.Start();

        _context.Save(data);

        transaction.Commit();
    }
}
