using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nice3point.Revit.AddIn.ViewModels;

public sealed class Nice3point.Revit.AddInViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}