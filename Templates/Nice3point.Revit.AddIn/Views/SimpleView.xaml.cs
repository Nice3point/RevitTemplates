using Nice3point.Revit.AddIn.ViewModels;

namespace Nice3point.Revit.AddIn.Views
{
    public partial class SimpleView
    {
        public SimpleView(SimpleViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}