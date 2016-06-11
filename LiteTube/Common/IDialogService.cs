using LiteTube.Common.Helpers;
using System;

namespace LiteTube.Common
{
    interface IDialogService
    {
        void ShowError(string message);
        void ShowException(Exception exception);
    }

    class DialogService : IDialogService
    {
        public void ShowError(string message)
        {
            LayoutHelper.InvokeFromUiThread(() =>
            {
#if SILVERLIGHT
                ExceptionDialog.ShowDialog(message);
#else
            throw new NotImplementedException();
#endif
            });
        }

        public void ShowException(Exception exception)
        {
            LayoutHelper.InvokeFromUiThread(() =>
            {
#if SILVERLIGHT
                ExceptionDialog.ShowError(exception);
#else
            throw new NotImplementedException();
#endif
            });
        }
    }
}
