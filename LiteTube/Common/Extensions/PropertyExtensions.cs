using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;

namespace LiteTube.Common
{
    public static class PropertyExtensions
    {
        public static void NotifyOfPropertyChanged<T>(this INotifyPropertyChanged sender, Expression<Func<T>> propertyExpression, PropertyChangedEventHandler eventHandler)
        {
            if (eventHandler == null)
                return;

            var body = (MemberExpression)propertyExpression.Body;
            eventHandler(sender, new PropertyChangedEventArgs(body.Member.Name));
        }
    }
}
