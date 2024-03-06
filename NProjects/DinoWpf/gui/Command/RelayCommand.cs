using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DinoWpf.Command
{
    public class RelayCommand<T> : ICommand
    {
        private Predicate<T> _canExecute;
        private Action<T> _execute;

        public Predicate<T> CanExecuteDelegate 
        {
            get { return _canExecute; }
            set { _canExecute = value; }
        }
        public Action<T> ExecuteDelegate 
        {
            get { return _execute; }
            set { _execute = value; }
        }

        public RelayCommand(Predicate<T> canExecute, Action<T> execute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _canExecute = canExecute;
            _execute = execute;
        }
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }
        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
    public class RelayCommand : ICommand
    {
        private Predicate<object> _canExecute;
        private Action<object> _execute;

        public Predicate<object> CanExecuteDelegate
        {
            get { return _canExecute; }
            set { _canExecute = value; }
        }
        public Action<object> ExecuteDelegate
        {
            get { return _execute; }
            set { _execute = value; }
        }

        public RelayCommand(Predicate<object> canExecute = null, Action<object> execute = null)
        {
            //if (execute == null)
            //    throw new ArgumentNullException("execute");
            _canExecute = canExecute;
            _execute = execute;
        }
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((object)parameter);
        }
        public void Execute(object parameter)
        {
            _execute((object)parameter);
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
