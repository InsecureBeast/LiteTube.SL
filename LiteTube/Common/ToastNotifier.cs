using LiteTube.Common.Helpers;
using Microsoft.Phone.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LiteTube.Common
{
    class ToastNotifier : IDisposable
    {
        private HttpNotificationChannel _pushChannel;
        private readonly string _channelName = "ToastLighTubeChannel";

        public ToastNotifier()
        {
            CreateChannel();   
        }

        public void Notify()
        {
        }

        private void CreateChannel()
        {
            // Try to find the push channel.
            _pushChannel = HttpNotificationChannel.Find(_channelName);

            // If the channel was not found, then create a new connection to the push service.
            if (_pushChannel == null)
            {
                _pushChannel = new HttpNotificationChannel(_channelName);

                // Register for all the events before attempting to open the channel.
                _pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                _pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                _pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);
                _pushChannel.Open();

                // Bind this new channel for toast events.
                _pushChannel.BindToShellToast();

            }
            else
            {
                // The channel was already open, so just register for all the events.
                _pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                _pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                _pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                // Display the URI for testing purposes. Normally, the URI would be passed back to your web service at this point.
                //System.Diagnostics.Debug.WriteLine(_pushChannel.ChannelUri.ToString());
                //MessageBox.Show(String.Format("Channel Uri is {0}", _pushChannel.ChannelUri.ToString()));
            }
        }

        private void PushChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            StringBuilder message = new StringBuilder();
            string relativeUri = string.Empty;

            message.AppendFormat("Received Toast {0}:\n", DateTime.Now.ToShortTimeString());

            // Parse out the information that was part of the message.
            foreach (string key in e.Collection.Keys)
            {
                message.AppendFormat("{0}: {1}\n", key, e.Collection[key]);

                if (string.Compare(
                    key,
                    "wp:Param",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.CompareOptions.IgnoreCase) == 0)
                {
                    relativeUri = e.Collection[key];
                }
            }

            // Display a dialog of all the fields in the toast.
            LayoutHelper.InvokeFromUiThread(() => MessageBox.Show(message.ToString()));
        }

        private void PushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {

        }

        private void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {

        }

        public void Dispose()
        {
            _pushChannel.Dispose();
        }
    }
}
