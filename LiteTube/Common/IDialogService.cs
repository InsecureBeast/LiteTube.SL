using LiteTube.Common.Helpers;
using System;

namespace LiteTube.Common
{
    interface IDialogService
    {
        void ShowError(Exception exception);
        void ShowException(Exception exception);
    }

    class DialogService : IDialogService
    {
        public void ShowError(Exception exception)
        {
            LayoutHelper.InvokeFromUiThread(() =>
            {
#if SILVERLIGHT
                ExceptionDialog.ShowDialog(exception);
#endif
                throw new NotImplementedException();
            });
        }

        public void ShowException(Exception exception)
        {
            LayoutHelper.InvokeFromUiThread(() =>
            {
#if SILVERLIGHT
                ExceptionDialog.ShowError(exception);
#endif
                throw new NotImplementedException();
            });
        }
    }
}
