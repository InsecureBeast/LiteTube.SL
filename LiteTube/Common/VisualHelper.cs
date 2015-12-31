using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LiteTube.Common
{
    internal class VisualHelper
    {
        // method to pull out a ScrollViewer
        public static ScrollViewer GetScrollViewer(DependencyObject depObj)
        {
            if (depObj is ScrollViewer) 
                return depObj as ScrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                var result = GetScrollViewer(child);
                if (result != null) 
                    return result;
            }

            return null;
        }

        public static T FindParent<T>(FrameworkElement element) where T : DependencyObject
        {
            DependencyObject current = element;
            while (current != null && !(current is T))
            {
                var visualParent = VisualTreeHelper.GetParent(current);
                if (visualParent == null)
                {
                    var fe = current as FrameworkElement;
                    if (fe != null)
                        visualParent = fe.Parent;
                }
                current = visualParent;
            }
            return current as T;
        }

        public static T FindChild<T>(FrameworkElement element) where T : DependencyObject
        {
            int childNumber = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childNumber; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                if (child != null && child is T)
                    return child as T;
                else
                {
                    child = FindChild<T>(child as FrameworkElement);
                    if (child != null && child is T)
                        return child as T;
                }
            }
            return null;
        }
    }
}
