namespace Fidl.Helpers
{
    using System;
    using System.Windows.Input;

    internal class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;

        private readonly Predicate<T> _canExecute;

        internal RelayCommand(Action<T> execute) : this(execute, _ => true)
        {
        }

        internal RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;

            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute((T)parameter);
        }
    }
}