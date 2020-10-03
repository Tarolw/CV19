using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace CV19.Infrastructure.Commands.Base
{
    internal abstract class Command : ICommand
    {
        // вызывается в случае изменения состояния команды
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /* Если возвращает ложь, то команду выполнить нельзя
         * и элемент к которому привязана команда отключается
         */
        public abstract bool CanExecute(object parameter);

        // Основная логика команды
        public abstract void Execute(object parameter);
    }
}
