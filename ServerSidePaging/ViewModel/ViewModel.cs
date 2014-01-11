using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ServerSidePaging.ViewModel
{
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;

    public abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaiseAllPropertiesChanged()
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(string.Empty));
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(propertyExpression.GetPropertyName()));
        }

        protected void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }

        public void NotifyLoaded()
        {
            OnViewLoaded();
        }

        public void NotifyUnloaded()
        {
            OnViewUnloaded();
        }

        protected virtual void OnViewLoaded()
        {

        }

        protected virtual void OnViewUnloaded()
        {

        }
    }

    public static class SymbolExtensions
    {
        /// <summary>
        /// Gets the PropertyInfo for the last property access in an expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<T>(this Expression<Func<T>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new InvalidOperationException("expression must consist of a property access");
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new InvalidOperationException("expression must consist of a property access");
            }

            return propertyInfo;
        }

        public static string GetPropertyName<T>(this Expression<Func<T>> expression)
        {
            return GetPropertyInfo(expression).Name;
        }
    }
}
