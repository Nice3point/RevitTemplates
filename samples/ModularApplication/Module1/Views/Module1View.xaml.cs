using Module1.ViewModels;

namespace Module1.Views;

public sealed partial class Module1View
{
    public Module1View(Module1ViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}