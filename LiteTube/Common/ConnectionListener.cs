using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Net.NetworkInformation;

namespace LiteTube.Common
{
    public sealed class ConnectionEventArgs : EventArgs
    {
        public bool IsConnected { get; private set; }

        public ConnectionEventArgs(bool isConnected)
        {
            IsConnected = isConnected;
        }
    }

    public class ConnectionListener
    {
        private readonly Notifier<ConnectionEventArgs> _notifier = new Notifier<ConnectionEventArgs>();

        public ConnectionListener()
        {
            // Subscribe to the NetworkAvailabilityChanged event
            DeviceNetworkInformation.NetworkAvailabilityChanged += ChangeDetected;

        }

        public void Subscribe(IListener<ConnectionEventArgs> listener)
        {
            _notifier.Subscribe(listener);
        }
    
        public static bool CheckNetworkAvailability()
        {
            // this is coming true even when i disconnected my pc from internet.
            // i also make the dataconnection off of the emulator
            var fg = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

            var ni = NetworkInterface.NetworkInterfaceType;
            // this part is coming none  
            return ni != NetworkInterfaceType.None;
        }

        private void ChangeDetected(object sender, NetworkNotificationEventArgs e)
        {
            ConnectionEventArgs connectionEventArgs;
            switch (e.NotificationType)
            {
                case NetworkNotificationType.InterfaceConnected:
                    connectionEventArgs = new ConnectionEventArgs(true);
                    _notifier.Notify(connectionEventArgs);
                    break;
                case NetworkNotificationType.InterfaceDisconnected:
                    connectionEventArgs = new ConnectionEventArgs(false);
                    _notifier.Notify(connectionEventArgs);
                    break;
                case NetworkNotificationType.CharacteristicUpdate:
                    break;
                default:
                    break;
            }
        }
    }
}
