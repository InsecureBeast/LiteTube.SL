using System;
using System.Net;
using System.Windows;
using LiteTube.Common.Helpers;
using Microsoft.Phone.Controls;
using LiteTube.Resources;
using LiteTube.Common.Tools;

namespace LiteTube.Common
{
    class ExceptionDialog
    {
        private static Exception _exception;

        public static void ShowError(Exception exception)
        {
            _exception = exception;

            // Show the message dialog
            var messageBox = new CustomMessageBox
            {
                Message = GetMessage(exception),
                Caption = GetTitle(),
                RightButtonContent = GetCloseCommandCaption(),
                LeftButtonContent = GetSendCommandCaption(),
                Foreground = ThemeManager.InverseForegroundBrush
            };

            messageBox.Dismissed += MessageBoxOnDismissed;
            messageBox.Show();
        }

        private static async void MessageBoxOnDismissed(object sender, DismissedEventArgs e)
        {
            switch (e.Result)
            {
                case CustomMessageBoxResult.LeftButton:
                    // Add the following task here which you wish to perform on speak button
                    var region = SettingsHelper.GetRegion();
                    await BugTreckerReporter.SendException(_exception, new[] { region });

                    break;
                case CustomMessageBoxResult.RightButton:
                    // Do something.

                    break;
                case CustomMessageBoxResult.None:
                    // Do something.

                    break;
                default:
                    break;
            }
        }

        private static string GetTitle()
        {
            return AppResources.ExceptionTitle;
        }

        private static string GetSendCommandCaption()
        {
            return AppResources.Send;
        }

        private static string GetCloseCommandCaption()
        {
            return AppResources.Close;
        }

        private static string GetMessage(Exception exception)
        {
            var ex = exception as WebException;
            if (ex == null)
                return exception.Message;

            if (ex.Status == WebExceptionStatus.ConnectFailure || (int)ex.Status == 1)
                return AppResources.CheckConnection;

            return ex.Message; 
        }

        public static void ShowDialog(string message)
        {
            MessageBox.Show(message, AppResources.ErrorTitle, MessageBoxButton.OK);
        }
    }
}
