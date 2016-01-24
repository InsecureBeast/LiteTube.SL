using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LiteTube.Controls
{
    public class NavigationButton : Button
    {
        public static readonly DependencyProperty IconProperty =
           DependencyProperty.Register("Icon", typeof(object), typeof(NavigationButton), new PropertyMetadata(null));

        public static readonly DependencyProperty TextProperty =
           DependencyProperty.Register("Text", typeof(string), typeof(NavigationButton), new PropertyMetadata(null));

        public object Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}
