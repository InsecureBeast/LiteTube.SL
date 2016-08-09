using System.Windows;
using System.Windows.Controls;

namespace LiteTube.Core.Common.Extensions
{
    internal static class ControlExtensions
    {
        public static void GoToVisualState(this Control control, string state)
        {
            control.GoToVisualState(state, true);
        }

        public static void GoToVisualState(this Control control, string state, bool useTransitions)
        {
            VisualStateManager.GoToState(control, state, useTransitions);
        }
    }
}
