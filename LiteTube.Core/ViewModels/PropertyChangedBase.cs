using System;
using System.ComponentModel;
using System.Linq.Expressions;
using LiteTube.Core.Common.Extensions;

namespace LiteTube.Core.ViewModels
{
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyOfPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            this.NotifyOfPropertyChanged(propertyExpression, PropertyChanged);
        }

        #endregion
    }
}
