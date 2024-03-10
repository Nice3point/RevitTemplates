using RevitAddIn.ViewModels;

namespace RevitAddIn.Views;

public sealed partial class RevitAddInView
{
    public RevitAddInView(RevitAddInViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}