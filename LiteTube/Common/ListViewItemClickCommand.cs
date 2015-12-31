using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace LiteTube.Common
{
    public static class ListViewItemClickCommand
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(ListViewItemClickCommand), new PropertyMetadata(null, OnCommandPropertyChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(ListViewItemClickCommand), new PropertyMetadata(null));

        public static void SetCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(CommandProperty);
        }

        public static void SetCommandParameter(DependencyObject d, object value)
        {
            d.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(DependencyObject d)
        {
            return d.GetValue(CommandParameterProperty);
        }

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ListPickerItem;
            if (control != null)
                control.Tap += Control_Tapped;
        }

        private static void Control_Tapped(object sender, GestureEventArgs e)
        {
            var control = sender as ListPickerItem;
            var command = GetCommand(control);
            var param = GetCommandParameter(control);

            if (command != null && command.CanExecute(null))
                command.Execute(control);
        }
    }
}
