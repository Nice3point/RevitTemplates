using System;
using System.Windows.Input;

namespace Nice3point.FrameworkAddIn.ViewModel.Objects
{
    public class RelayCommand : ICommand
    {
        private readonly bool _canExecute;
        private readonly Action<object> _execute;

        public RelayCommand(Action<object> execute, bool canExecute = true)
        {
            _execute    = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}