using ModalModule.ViewModels;

namespace ModalModule.Views;

/// <summary>
///     Represents the add-in window.
/// </summary>
public sealed partial class ModalModuleView
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ModalModuleView" /> class.
    /// </summary>
    /// <param name="viewModel">The view model associated with the window.</param>
    public ModalModuleView(ModalModuleViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
