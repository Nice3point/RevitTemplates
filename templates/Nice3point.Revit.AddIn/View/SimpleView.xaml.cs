using Nice3point.Revit.AddIn.ViewModel;

namespace Nice3point.Revit.AddIn.View
{
    public partial class SimpleView
    {
        private readonly SimpleViewModel _viewModel;

        public SimpleView(SimpleViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}