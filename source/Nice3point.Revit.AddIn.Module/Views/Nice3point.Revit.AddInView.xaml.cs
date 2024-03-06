using Nice3point.Revit.AddIn.ViewModels;

namespace Nice3point.Revit.AddIn.Views;

public sealed partial class Nice3point.Revit.AddInView
{
    public Nice3point.Revit.AddInView(Nice3point.Revit.AddInViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}