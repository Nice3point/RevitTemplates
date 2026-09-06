using Nice3point.Revit.AddIn._1.ViewModels;

namespace Nice3point.Revit.AddIn._1.Views;

/// <summary>
///     Represents the add-in window.
/// </summary>
public sealed partial class Nice3point_Revit_AddIn__1View
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Nice3point_Revit_AddIn__1View" /> class.
    /// </summary>
    /// <param name="viewModel">The view model associated with the window.</param>
    public Nice3point_Revit_AddIn__1View(Nice3point_Revit_AddIn__1ViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
