using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;
using LiteTube.Common;

namespace LiteTube.ViewModels
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
