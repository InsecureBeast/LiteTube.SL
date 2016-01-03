using System.Linq;
using System.Windows.Navigation;

namespace LiteTube.Common.Helpers
{
    static class NavigationHelper
    {
        public static void GoHome()
        {
            var list = App.RootFrame.BackStack.ToList();
            for (int i = 0; i < list.Count - 1; i++)
            {
                App.RootFrame.RemoveBackEntry();
            }

            App.RootFrame.GoBack();
        }
    }
}
