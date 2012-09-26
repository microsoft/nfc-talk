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

        private enum ConnectionStatusValue
        {
            NotConnected = 0,
            Searching,
            Connecting,
            Listening,
            Connected
        };

        private ConnectionStatusValue _status;
        private StreamSocket _socket;
        private DataWriter _writer;
        private DataReader _reader;
        private string _name;

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

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        if (PropertyChanged != null)
                        {
                            PropertyChanged(this, new PropertyChangedEventArgs("PeerName"));
                        }
                    });
                }
            }
        }

        public void Connect()
        {
            if (_status == ConnectionStatusValue.NotConnected)
            {
                _status = ConnectionStatusValue.Searching;

                PeerName = "";

                PeerFinder.DisplayName = NFCTalk.DataContext.Singleton.Settings.Name;
                PeerFinder.TriggeredConnectionStateChanged += TriggeredConnectionStateChanged;

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
                    {
                        PeerFinder.Stop();
                        PeerFinder.TriggeredConnectionStateChanged -= TriggeredConnectionStateChanged;
                        _status = ConnectionStatusValue.NotConnected;
                    }
                    break;

                case ConnectionStatusValue.Connected:
                    {
                        if (_socket != null)
                        {
                            _socket.Dispose();
                        }

                        _status = ConnectionStatusValue.NotConnected;
                    }
                    break;
            }
        }

        private void TriggeredConnectionStateChanged(object sender, TriggeredConnectionStateChangedEventArgs e)
        {
            switch (e.State)
            {
                case TriggeredConnectState.PeerFound: System.Diagnostics.Debug.WriteLine("PeerFound");
                    {
                        if (Connecting != null)
                        {
                            Connecting();
                        }
                    }
                    break;

                case TriggeredConnectState.Connecting: System.Diagnostics.Debug.WriteLine("Connecting");
                    {
                        _status = ConnectionStatusValue.Connecting;
                    }
                    break;

                case TriggeredConnectState.Listening: System.Diagnostics.Debug.WriteLine("Listening");
                    {
                        _status = ConnectionStatusValue.Listening;
                    }
                    break;

                case TriggeredConnectState.Completed: System.Diagnostics.Debug.WriteLine("Completed");
                    {
                        PeerFinder.Stop();
                        PeerFinder.TriggeredConnectionStateChanged -= TriggeredConnectionStateChanged;
                        _socket = e.Socket;
                        _writer = new DataWriter(e.Socket.OutputStream);
                        _writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                        _writer.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;
                        _reader = new DataReader(e.Socket.InputStream);
                        _reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                        _reader.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;
                        _status = ConnectionStatusValue.Connected;

                        ListenAsync();

                        SendNameAsync(NFCTalk.DataContext.Singleton.Settings.Name);

                        if (Connected != null)
                        {
                            Connected();
                        }
                    }
                    break;

                case TriggeredConnectState.Canceled: System.Diagnostics.Debug.WriteLine("Canceled");
                    {
                        if (UnableToConnect != null)
                        {
                            UnableToConnect();
                        }
                    }
                    break;

                case TriggeredConnectState.Failed: System.Diagnostics.Debug.WriteLine("Failed");
                    {
                        if (UnableToConnect != null)
                        {
                            UnableToConnect();
                        }
                    }
                    break;
            }
        }

        private async Task SendNameAsync(string name)
        {
            try
            {
                _writer.WriteUInt32(0); // protocol version
                _writer.WriteUInt32(0); // operation identifier

                uint length = _writer.MeasureString(name);
                _writer.WriteUInt32(length);
                _writer.WriteString(name);

                await _writer.StoreAsync();
            }
            catch (Exception)
            {
                if (ConnectionInterrupted != null)
                {
                    ConnectionInterrupted();
                }
            }
        }

        public async Task SendMessageAsync(Message m)
        {
            try
            {
                if (m.Text.Length > 0)
                {
                    _writer.WriteUInt32(0); // protocol version
                    _writer.WriteUInt32(1); // operation identifier

                    uint length = _writer.MeasureString(m.Text);
                    _writer.WriteUInt32(length);
                    _writer.WriteString(m.Text);

                    await _writer.StoreAsync();
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
                    uint version = _reader.ReadUInt32();

                    if (version == 0)
                    {
                        await GuaranteedLoadAsync(sizeof(UInt32));
                        uint operation = _reader.ReadUInt32();

                        switch (operation)
                        {
                            case 0: // name
                                {
                                    await GuaranteedLoadAsync(sizeof(UInt32));
                                    uint length = _reader.ReadUInt32();
                                    await GuaranteedLoadAsync(length);
                                    string name = _reader.ReadString(length);

                                    PeerName = name;
                                }
                                break;

                            case 1: // message
                                {
                                    await GuaranteedLoadAsync(sizeof(UInt32));
                                    uint length = _reader.ReadUInt32();

                                    await GuaranteedLoadAsync(length);
                                    string text = _reader.ReadString(length);

                                    Message m = new Message()
                                    {
                                        Name = PeerName,
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
                    else
                    {
                        throw new Exception("Protocol version mismatch");
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
    }
}