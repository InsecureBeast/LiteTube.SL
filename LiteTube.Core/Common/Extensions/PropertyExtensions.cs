using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace LiteTube.Core.Common.Extensions
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
