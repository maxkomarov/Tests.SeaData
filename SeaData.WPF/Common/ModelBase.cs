using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace SeaData.WPF.Common
{
    /// <summary>
    /// Суперкласс для всех классов модели
    /// </summary>
    public abstract class ModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region имплементация INotifyPropertyChanged
        /// <summary>
        /// Происходит при изменении значения какого-либо свойства экземпляра класса
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Вызывает событие PropertyChanged у подписчиков
        /// </summary>
        /// <param name="expression">измененное свойство (как выражение)</param>
        protected void OnPropertyChanged(Expression<Func<object>> expression)
        {
           string propertyName = PropertyName.For(expression);
           PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Имплементация INotifyDataErrorInfo
        private Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (errors.ContainsKey(propertyName))
                return errors[propertyName];
            return null;
        }

        public bool HasErrors { get => errors.Count > 0; }

        public bool IsValid {get => !HasErrors; }
        
        public void AddError(Expression<Func<object>> expression, string error)
        {
            string propertyName = PropertyName.For(expression);
            errors[propertyName] = new List<string>() { error };
            OnNotifyErrorsChanged(propertyName);
        }

        public void RemoveError(Expression<Func<object>> expression)
        {
            string propertyName = PropertyName.For(expression);
            if (errors.ContainsKey(propertyName))
                errors.Remove(propertyName);
            OnNotifyErrorsChanged(propertyName);
        }

        /// <summary>
        /// Вызывавет событие ErrorsChanged у подписчиков
        /// </summary>
        /// <param name="propertyName">Имя свойства, состояние ошибки которого изменилось</param>
        public void OnNotifyErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
        #endregion
    }
}
