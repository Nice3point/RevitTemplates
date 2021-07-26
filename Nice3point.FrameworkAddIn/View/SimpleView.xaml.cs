using Nice3point.FrameworkAddIn.ViewModel;

namespace Nice3point.FrameworkAddIn.View
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