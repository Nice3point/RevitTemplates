using Module2.ViewModels;

namespace Module2.Views;

/// <summary>
///     Represents the add-in window.
/// </summary>
public sealed partial class Module2View
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Module2View" /> class.
    /// </summary>
    /// <param name="viewModel">The view model associated with the window.</param>
    public Module2View(Module2ViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
