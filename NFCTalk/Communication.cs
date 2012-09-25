using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace NFCTalk
{
    class Communication : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Action Connecting;
        public Action Connected;
        public Action UnableToConnect;
        public Action ConnectionInterrupted;
        public Action<Message> MessageReceived;

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
        DataWriter _writer;
        DataReader _reader;
        string _name;

        public string PeerName
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name != value)
                {
                    _name = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("PeerName"));
                    }
                }
            }
        }

        public void Connect()
        {
            if (_status == ConnectionStatusValue.NotConnected)
            {
                PeerName = "";

                PeerFinder.DisplayName = NFCTalk.DataContext.Singleton.Settings.Name;
                PeerFinder.TriggeredConnectionStateChanged += TriggeredConnectionStateChanged;
                //PeerFinder.ConnectionRequested += ConnectionRequested;

                _status = ConnectionStatusValue.Searching;

                PeerFinder.Start();
            }
            else
            {
                throw new Exception("Bad state, please disconnect first");
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
                case TriggeredConnectState.PeerFound: System.Diagnostics.Debug.WriteLine("PeerFound");
                    if (Connecting != null)
                    {
                        Connecting();
                    }
                    break;

                case TriggeredConnectState.Connecting: System.Diagnostics.Debug.WriteLine("Connecting");
                    _status = ConnectionStatusValue.Connecting;
                    break;

                case TriggeredConnectState.Listening: System.Diagnostics.Debug.WriteLine("Listening");
                    _status = ConnectionStatusValue.Listening;
                    break;

                case TriggeredConnectState.Completed: System.Diagnostics.Debug.WriteLine("Completed");
                    PeerFinder.Stop();
                    PeerFinder.TriggeredConnectionStateChanged -= TriggeredConnectionStateChanged;
                    _socket = e.Socket;
                    _writer = new DataWriter(e.Socket.OutputStream);
                    _reader = new DataReader(e.Socket.InputStream);
                    _status = ConnectionStatusValue.Connected;

                    ListenAsync();

                    SendNameAsync(NFCTalk.DataContext.Singleton.Settings.Name);

                    if (Connected != null)
                    {
                        Connected();
                    }
                    break;

                case TriggeredConnectState.Canceled: System.Diagnostics.Debug.WriteLine("Canceled");
                    if (UnableToConnect != null)
                    {
                        UnableToConnect();
                    }
                    break;

                case TriggeredConnectState.Failed: System.Diagnostics.Debug.WriteLine("Failed");
                    if (UnableToConnect != null)
                    {
                        UnableToConnect();
                    }
                    break;
            }
        }
        
        async Task SendNameAsync(string name)
        {
            _writer.WriteUInt32(0);

            uint length = _writer.MeasureString(name);
            _writer.WriteUInt32(length);
            _writer.WriteString(name);

            await _writer.StoreAsync();
        }

        public async Task SendMessageAsync(Message m)
        {
            if (m.Text.Length > 0)
            {
                uint length;

                _writer.WriteUInt32(1);

                length = _writer.MeasureString(m.Text);
                _writer.WriteUInt32(length);
                _writer.WriteString(m.Text);

                await _writer.StoreAsync();
            }
        }

        private async Task GuaranteedLoadAsync(uint length)
        {
            DataReaderLoadOperation op;

            while (length != _reader.UnconsumedBufferLength)
            {
                op = _reader.LoadAsync(length - _reader.UnconsumedBufferLength);

                if (await op.AsTask<uint>() == 0)
                {
                    throw new Exception();
                }
            }
        }

        private async Task ListenAsync()
        {
            try
            {
                while (true)
                {
                    await GuaranteedLoadAsync(sizeof(UInt32));
                    uint code = _reader.ReadUInt32();

                    switch (code)
                    {
                        case 0: // name
                            {
                                await GuaranteedLoadAsync(sizeof(UInt32));
                                uint length = _reader.ReadUInt32();
                                await GuaranteedLoadAsync(length);
                                string name = _reader.ReadString(length);

                                Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    PeerName = name;
                                });
                            }
                            break;

                        case 1: // message
                            {
                                uint length;

                                await GuaranteedLoadAsync(sizeof(UInt32));
                                length = _reader.ReadUInt32();

                                await GuaranteedLoadAsync(length);
                                string text = _reader.ReadString(length);

                                Message m = new Message()
                                {
                                    Name = _name,
                                    Text = text,
                                    Direction = Message.DirectionValue.In
                                };

                                if (MessageReceived != null)
                                {
                                    MessageReceived(m);
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception)
            {
                if (ConnectionInterrupted != null)
                {
                    ConnectionInterrupted();
                }
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
    }
}