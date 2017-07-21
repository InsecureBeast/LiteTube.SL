using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SM.Media.Core.Web.HttpConnection
{
    public sealed class StreamSocketWrapper : ISocket, IDisposable
    {
        private StreamSocket _socket;

        public void Dispose()
        {
            this.Close();
        }

        public async Task ConnectAsync(Uri url, CancellationToken cancellationToken)
        {
            string host = url.Host;
            string serviceName = url.Port.ToString((IFormatProvider)CultureInfo.InvariantCulture);
            HostName hostName = new HostName(host);
            bool useTls = string.Equals("HTTPS", url.Scheme, StringComparison.OrdinalIgnoreCase);
            if (!useTls && !string.Equals("HTTP", url.Scheme, StringComparison.OrdinalIgnoreCase))
                throw new NotSupportedException("Scheme not supported: " + url.Scheme);
            StreamSocket socket = new StreamSocket();
            if (null != Interlocked.CompareExchange<StreamSocket>(ref this._socket, socket, (StreamSocket)null))
            {
                socket.Dispose();
                throw new InvalidOperationException("The socket is in use");
            }
            try
            {
                socket.Control.NoDelay = true;
                SocketProtectionLevel protectionLevel = useTls ? SocketProtectionLevel.Ssl : SocketProtectionLevel.PlainSocket;
                await WindowsRuntimeSystemExtensions.AsTask(socket.ConnectAsync(hostName, serviceName, protectionLevel), cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Close();
                throw;
            }
        }

        public async Task<int> WriteAsync(byte[] buffer, int offset, int length, CancellationToken cancellationToken)
        {
            StreamSocket socket = this._socket;
            if (null == socket)
                throw new InvalidOperationException("The socket is not open");
            IBuffer iBuffer = WindowsRuntimeBufferExtensions.AsBuffer(buffer, offset, length);
            return (int)await WindowsRuntimeSystemExtensions.AsTask<uint, uint>(socket.OutputStream.WriteAsync(iBuffer), cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> ReadAsync(byte[] buffer, int offset, int length, CancellationToken cancellationToken)
        {
            StreamSocket socket = this._socket;
            if (null == socket)
                throw new InvalidOperationException("The socket is not open");
            IBuffer iBuffer = WindowsRuntimeBufferExtensions.AsBuffer(buffer, offset, 0, length);
            IBuffer iBuffer2 = await WindowsRuntimeSystemExtensions.AsTask<IBuffer, uint>(socket.InputStream.ReadAsync(iBuffer, (uint)length, InputStreamOptions.Partial), cancellationToken).ConfigureAwait(false);
            int bytesRead = (int)iBuffer2.Length;
            int num;
            if (bytesRead <= 0)
                num = 0;
            else if (object.ReferenceEquals((object)iBuffer, (object)iBuffer2))
            {
                num = bytesRead;
            }
            else
            {
                Debug.Assert(bytesRead <= length, "Length out-of-range");
                WindowsRuntimeBufferExtensions.CopyTo(iBuffer2, 0U, buffer, offset, bytesRead);
                num = bytesRead;
            }
            return num;
        }

        public void Close()
        {
            StreamSocket streamSocket = Interlocked.Exchange<StreamSocket>(ref this._socket, (StreamSocket)null);
            if (null == streamSocket)
                return;
            try
            {
                streamSocket.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("StreamSocketWrapper.Close() failed " + ex.Message);
            }
        }
    }
}
