using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace CV19.ViewModels.Base
{
    internal abstract class ViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /* Метод вызывающий событие PropertyChanged
         * [CallerMemberName] автоматически подставит имя свойства или метода
         * (из котороко происходит вызов) в PropertyName 
         */
        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)   
        {
            // Генерируем событие
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        /* Метод обновляющий значение свойства (ref T field - сылка на поле свойства), 
         * T value - новое значение, которое мы хотим установить
         * [CallerMemberName] string PropertyName = null - сюда компилятор автоматически запихнет имя нашего свойства
         */
        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            // Если значение, которое мы хотим обновить соответствует текущему то false
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }

        //~ViewModel()
        //{
        //    Dispose(false);
        //}

        public void Dispose()
        {
            Dispose(true);
        }

        private bool _Disposed;
        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposing || _Disposed) return;
            _Disposed = true;
            // Освобождение управляемых ресурсов
        }
    }
}
