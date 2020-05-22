using System;
using Windows.Networking.Sockets;

namespace SoundByte.App.Uwp.Extensions.Definitions.NetworkDef
{
    public class WebSocket
    {
        #region Private Variables

        private MessageWebSocket _messageWebSocket;

        #endregion Private Variables

        #region Getters and Setters

        public string Url { get; private set; }

        #endregion Getters and Setters

        public WebSocket(string address)
        {
            _messageWebSocket = new MessageWebSocket();
            Url = address;
        }

        public void Connect()
        {
            _messageWebSocket.ConnectAsync(new Uri(Url)).AsTask().Wait();
        }

        public void Send()
        {
        }

        public void Close(int? code = null, string reason = null)
        {
            _messageWebSocket.Close((ushort)code, reason);
        }
    }
}