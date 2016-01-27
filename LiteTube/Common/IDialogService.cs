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
        private readonly IDeviceHistory _deviceHistory;

        public DialogService(IDeviceHistory deviceHistory)
        {
            _deviceHistory = deviceHistory;
        }

        public void ShowError(Exception exception)
        {
            ExceptionDialog.ShowDialog(exception);
        }

        public void ShowException(Exception exception)
        {
            ExceptionDialog.ShowError(exception);
        }
    }
}
