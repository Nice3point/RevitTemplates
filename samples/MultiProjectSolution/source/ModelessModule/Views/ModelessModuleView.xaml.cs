using ModelessModule.ViewModels;

namespace ModelessModule.Views;

public sealed partial class ModelessModuleView
{
    public ModelessModuleView(ModelessModuleViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}