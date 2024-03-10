using ModalModule.ViewModels;

namespace ModalModule.Views;

public sealed partial class ModalModuleView
{
    public ModalModuleView(ModalModuleViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}