using Module2.ViewModels;

namespace Module2.Views;

public sealed partial class Module2View
{
    public Module2View(Module2ViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}