using System;
using LiteTube.Common;
using Microsoft.Phone.Net.NetworkInformation;

namespace LiteTube.DataModel
{
    public interface IConnectionListener
    {
        void Subscribe(IListener<ConnectionEventArgs> listener);
        void Notify(ConnectionEventArgs e);
        bool CheckNetworkAvailability();
    }
    
    public sealed class ConnectionEventArgs : EventArgs
    {
        public bool IsConnected { get; private set; }

        public ConnectionEventArgs(bool isConnected)
        {
            IsConnected = isConnected;
        }
    }

    public class ConnectionListener : IConnectionListener
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

        public void Notify(ConnectionEventArgs e)
        {
            _notifier.Notify(e);
        }

        public bool CheckNetworkAvailability()
        {
            // this is coming true even when i disconnected my pc from internet.
            // i also make the dataconnection off of the emulator
            var fg = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

            var ni = NetworkInterface.NetworkInterfaceType;
            // this part is coming none  
            return ni != NetworkInterfaceType.None;
        }

        protected virtual void OnConnectionChanged(ConnectionEventArgs e)
        {
        }

        private void ChangeDetected(object sender, NetworkNotificationEventArgs e)
        {
            ConnectionEventArgs connectionEventArgs;
            switch (e.NotificationType)
            {
                case NetworkNotificationType.InterfaceConnected:
                    connectionEventArgs = new ConnectionEventArgs(true);
                    OnConnectionChanged(connectionEventArgs);
                    break;
                case NetworkNotificationType.InterfaceDisconnected:
                    connectionEventArgs = new ConnectionEventArgs(false);
                    OnConnectionChanged(connectionEventArgs);
                    break;
                case NetworkNotificationType.CharacteristicUpdate:
                    break;
            }
        }
    }
}
