using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Module3;
using Module3.Enums;
using Nice3point.Revit.Toolkit;

namespace Module1.ViewModels;

public sealed partial class Module1ViewModel : ObservableObject
{
    [ObservableProperty] private string _projectName;

    public Module1ViewModel()
    {
        ProjectName = new DatabaseConnection(EntryKey.Data).Load<string>("ProjectName");
    }

    [RelayCommand]
    private void SaveProjectName()
    {
        var connection = new DatabaseConnection(EntryKey.Data);
        connection.BeginTransaction();

        connection.Save("ProjectName", ProjectName);

        connection.Close();
    }
}