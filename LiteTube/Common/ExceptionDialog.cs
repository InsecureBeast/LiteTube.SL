using System;
using System.Net;
using System.Windows;
using System.Windows.Media;
using Microsoft.Phone.Controls;

namespace LiteTube.Common
{
    class ExceptionDialog
    {
        private static Exception _exception;

        public static void ShowDialog(Exception exception)
        {
            // Show the message dialog
            var messageBox = new CustomMessageBox
            {
                Message = GetMessage(exception),
                Caption = GetTitle(),
                RightButtonContent = GetCloseCommandCaption(),
                LeftButtonContent = GetSendCommandCaption()
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
            //var resourceLoader = ResourceLoader.GetForCurrentView("Resources");
            //var str = resourceLoader.GetString("ExceptionPageTitle");
            //return str;
            return "Whoops! Something went wrong!";
        }

        private static string GetSendCommandCaption()
        {
            //var resourceLoader = ResourceLoader.GetForCurrentView("Resources");
            //var str = resourceLoader.GetString("SendErrorButton");
            //return str;
            return "Send";
        }

        private static string GetCloseCommandCaption()
        {
            //var resourceLoader = ResourceLoader.GetForCurrentView("Resources");
            //var str = resourceLoader.GetString("CloseErrorButton");
            //return str;
            return "Close";
        }

        private static string GetMessage(Exception exception)
        {
            var ex = exception as WebException;
            if (ex == null)
            {
                return exception.Message;
            }

            if (ex.Status == WebExceptionStatus.ConnectFailure || (int)ex.Status == 1)
            {
                //var resourceLoader = ResourceLoader.GetForCurrentView("Resources");
                //return resourceLoader.GetString("CheckConnectionMesage");
                return "Check connection";
            }

            return ex.Message; 
        }
    }
}
