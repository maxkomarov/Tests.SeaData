﻿using System;
using System.Windows.Input;

namespace SeaData.WPF.Common
{
    public class CommandBase : ICommand
    {
        readonly Action<object> execute;
        readonly Predicate<object> canExecute;

        public CommandBase(Action<object> executeDelegate, Predicate<object> canExecuteDelegate)
        {
            execute = executeDelegate;
            canExecute = canExecuteDelegate;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        void ICommand.Execute(object parameter)
        {
            execute(parameter);
        }
    }
}

