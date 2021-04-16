using CV19.Infrastructure.Commands;
using CV19.Models;
using CV19.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CV19.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {

        #region SelectedPageIndex : int - Номер выбранной вкладки

        ///<summary>Номер выбранной вкладки</summary>
        private int _selectedPageIndex;

        ///<summary>Номер выбранной вкладки</summary>
        public int SelectedPageIndex
        {
            get => _selectedPageIndex;
            set => Set(ref _selectedPageIndex, value);
        }

        #endregion

        #region TestDataPoints : IEnumerable<DataPoint> - Тестовый набор данных для визуализации графиков

        ///<summary>Тестовый набор данных для визуализации графиков</summary>
        private IEnumerable<DataPoint> _TestDataPoints;

        ///<summary>Тестовый набор данных для визуализации графиков</summary>
        public IEnumerable<DataPoint> TestDataPoints
        {
            get => _TestDataPoints;
            set => Set(ref _TestDataPoints, value);
        }

        #endregion

        #region Title : string - Заголовок окна

        /// <summary>Заголовок окна</summary>
        private string _Title = "Анализ статистики CV19";

        /// <summary>Заголовок окна</summary>
        public string Title
        {
            get => _Title;
            //set
            //{
            //    //if (Equals(_Title, value)) return;
            //    //_Title = value;
            //    //OnPropertyChanged();

            //    Set(ref _Title, value);
            //}
            set => Set(ref _Title, value);
        }

        #endregion

        #region Status : string - Статус программы

        ///<summary>Статус программы</summary>
        private string _Status = "Готов!";

        ///<summary>Статус программы</summary>
        public string Status 
        { 
            get => _Status; 
            set => Set(ref _Status, value); 
        }

        #endregion

        #region Команды

        #region CloseApplicationCommand

        // Свойство в которое мы помещаем команду
        public ICommand CloseApplicationCommand { get; }


        // В данном случае команда будет доступна для выполнения всегда, поэтому возвращаем true
        private bool CanCloseApplicationCommandExecute(object p) => true;

        // Этот метод будет выполняться, когда команда выполняется
        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region ChangeTabIndexCommand

        public ICommand ChangeTabIndexCommand { get; }

        private bool CanChangeTabIndexCommandExexute(object p) => !(_selectedPageIndex == 0 && Convert.ToInt32(p)==-1)&&!(_selectedPageIndex == 5 && Convert.ToInt32(p)==1);

        private void OnChangeTabIndexCommandExecuted(object p)
        {
            if (p is null) return;
            SelectedPageIndex += Convert.ToInt32(p);
        }

        #endregion

        #endregion

        public MainWindowViewModel()
        {
            #region Команды

            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            ChangeTabIndexCommand = new LambdaCommand(OnChangeTabIndexCommandExecuted, CanChangeTabIndexCommandExexute);

            #endregion

            var data_points = new List<DataPoint>((int)(360 / 0.1));
            for(var x = 0d; x <= 360; x += 0.1)
            {
                const double to_rad = Math.PI / 180;
                var y = Math.Sin(x * to_rad);

                data_points.Add(new DataPoint { XValue = x, YValue = y });
            }

            TestDataPoints = data_points;
        }
    }
}
