using Nice3point.Revit.AddIn.ViewModels;

namespace Nice3point.Revit.AddIn.Views;

public partial class Nice3point.Revit.AddInView
{
    public Nice3point.Revit.AddInView(Nice3point.Revit.AddInViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}