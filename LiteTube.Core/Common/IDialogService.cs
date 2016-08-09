using System;
using LiteTube.Core.Common.Helpers;

namespace LiteTube.Core.Common
{
    public interface IDialogService
    {
        void ShowError(string message);
        void ShowException(Exception exception);
    }

    public class DialogService : IDialogService
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
