using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;

namespace NFCTalk
{
    class Communication
    {
        enum ConnectionStatusValue
        {
            NotConnected = 0,
            Searching,
            Connecting,
            Listening,
            Connected
        }

        ConnectionStatusValue _status;
        StreamSocket _socket;

        public void Connect()
        {
            if (_status == ConnectionStatusValue.NotConnected)
            {
                //PeerFinder.DisplayName = NFCTalk.DataContext.Singleton().Settings.Name;
                PeerFinder.TriggeredConnectionStateChanged += TriggeredConnectionStateChanged;
                //PeerFinder.ConnectionRequested += ConnectionRequested;

                _status = ConnectionStatusValue.Searching;

                PeerFinder.Start();
            }
        }

        public void Disconnect()
        {
            switch (_status)
            {
                case ConnectionStatusValue.Searching:
                case ConnectionStatusValue.Connecting:
                case ConnectionStatusValue.Listening:
                    PeerFinder.Stop();
                    PeerFinder.TriggeredConnectionStateChanged -= TriggeredConnectionStateChanged;
                    _status = ConnectionStatusValue.NotConnected;
                    break;

                case ConnectionStatusValue.Connected:
                    if (_socket != null)
                    {
                        _socket.Dispose();
                    }

                    _status = ConnectionStatusValue.NotConnected;
                    break;
            }
        }

        void TriggeredConnectionStateChanged(object sender, TriggeredConnectionStateChangedEventArgs e)
        {
            switch (e.State)
            {
                case TriggeredConnectState.PeerFound:
                    // tap gesture is complete
                    break;

                case TriggeredConnectState.Connecting:
                    _status = ConnectionStatusValue.Connecting;
                    break;

                case TriggeredConnectState.Listening:
                    _status = ConnectionStatusValue.Listening;
                    break;

                case TriggeredConnectState.Completed:
                    PeerFinder.Stop();
                    PeerFinder.TriggeredConnectionStateChanged -= TriggeredConnectionStateChanged;
                    _socket = e.Socket;
                    _status = ConnectionStatusValue.Connected;
                    break;
            }
        }

        //private async void ConnectionRequested(object sender, ConnectionRequestedEventArgs e)
        //{
        //    switch (_status)
        //    {
        //        case ConnectionStatusValue.Connecting:
        //            _socket = await PeerFinder.ConnectAsync(e.PeerInformation);
        //            _status = ConnectionStatusValue.Connected;
        //            break;
        //    }
        //}

        //public int SendMessage(Message m);

        // event OnReceivedMessage(Message m);
        // event OnConnected();
        // event OnDisconnected();
        // event OnSendMessageComplete(int id);
        // event OnSendMessageFailed(int id);
    }
}
