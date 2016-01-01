using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Shell;
using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace LiteTube.Common
{
    public static class AppBarClickCommand
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(AppBarClickCommand), new PropertyMetadata(null, OnCommandPropertyChanged));

        public static ICommand _command;
            
        public static void SetCommand(IApplicationBarMenuItem d, Binding value)
        {
            var depObject = new Button();
            var exp1 = depObject.SetBinding(CommandProperty, value);
            BindingExpression exp = depObject.GetBindingExpression(CommandProperty);
            _command = depObject.Command;
        }

        public static ICommand GetCommand(DependencyObject d)
        {
            return _command;
        }

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as IApplicationBarMenuItem;
            if (control != null)
                control.Click += Control_Click;
        }

        private static void Control_Click(object sender, EventArgs e)
        {
            var control = sender as FrameworkElement;
            var command = GetCommand(control);

            if (control != null && command != null && command.CanExecute(null))
            {
                var page = VisualHelper.FindParent<Page>(control);
                command.Execute(page);
            }
        }
    }
}
