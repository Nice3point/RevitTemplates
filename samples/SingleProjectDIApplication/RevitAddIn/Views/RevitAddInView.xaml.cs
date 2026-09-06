using RevitAddIn.ViewModels;

namespace RevitAddIn.Views;

/// <summary>
///     Represents the add-in window.
/// </summary>
public sealed partial class RevitAddInView
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RevitAddInView" /> class.
    /// </summary>
    /// <param name="viewModel">The view model associated with the window.</param>
    public RevitAddInView(RevitAddInViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
