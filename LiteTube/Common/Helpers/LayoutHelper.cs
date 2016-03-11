using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace LiteTube.Common.Helpers
{
    public static class LayoutHelper
    {
        public static void InvokeFromUiThread(Action action)
        {
            Task.Run(async () =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync
                (
                    CoreDispatcherPriority.Normal, 
                    () => action()
                );
            });
        }
    }
}
