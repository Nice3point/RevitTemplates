using Module1.ViewModels;

namespace Module1.Views;

/// <summary>
///     Represents the add-in window.
/// </summary>
public sealed partial class Module1View
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Module1View" /> class.
    /// </summary>
    /// <param name="viewModel">The view model associated with the window.</param>
    public Module1View(Module1ViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
