using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LiteTube.Common.Helpers;
using Microsoft.Phone.Controls;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;
using LiteTube.ViewModels.Nodes;

namespace LiteTube.Common
{
    public static class ItemClickCommand
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(ItemClickCommand), new PropertyMetadata(null, OnCommandPropertyChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(ItemClickCommand), new PropertyMetadata(null));

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
            var control = d as Control;
            if (control != null)
                control.Tap += OnItemTap;
        }

        private static void OnItemTap(object sender, GestureEventArgs e)
        {
            var control = sender as Control;
            var command = GetCommand(control);
            //var selected = GetCommandParameter(control);

            if (control != null && command != null && command.CanExecute(null))
            {
                var source = e.OriginalSource as FrameworkElement;
                if (source == null)
                    return;

                var viewModel = source.DataContext as NodeViewModelBase;
                if (viewModel == null)
                {
                    command.Execute(null);
                    return;
                }

                var page = VisualHelper.FindParent<Page>(control);
                var navObj = new NavigationObject(viewModel, page);
                command.Execute(navObj);
            }
        }
    }
}
