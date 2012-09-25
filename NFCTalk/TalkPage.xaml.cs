using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Collections.Specialized;

namespace NFCTalk
{
    public partial class TalkPage : PhoneApplicationPage
    {
        NFCTalk.DataContext _dataContext = NFCTalk.DataContext.Singleton;

        void scrollToLast()
        {
            if (talkListBox.Items.Count > 0)
            {
                talkListBox.UpdateLayout();
                talkListBox.ScrollIntoView(talkListBox.Items[talkListBox.Items.Count - 1]);
            }
        }

        public TalkPage()
        {
            DataContext = _dataContext;
            
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            sendButton.IsEnabled = false;

            _dataContext.Communication.ConnectionInterrupted += ConnectionInterrupted;
            _dataContext.Communication.MessageReceived += MessageReceived;

            _dataContext.Messages.CollectionChanged += MessagesChanged;

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                scrollToLast();
            });
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            _dataContext.Communication.ConnectionInterrupted -= ConnectionInterrupted;
            _dataContext.Communication.MessageReceived -= MessageReceived;

            _dataContext.Messages.CollectionChanged -= MessagesChanged;

            _dataContext.Communication.Disconnect();

            foreach (Message m in _dataContext.Messages)
            {
                m.Archived = true;
            }

            base.OnNavigatingFrom(e);
        }

        private void MessagesChanged(object sender, EventArgs e)
        {
            scrollToLast();
        }

        private void MessageReceived(Message m)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                _dataContext.Messages.Add(m);
            });
        }

        private void ConnectionInterrupted()
        {
            _dataContext.Communication.Disconnect();

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.GoBack();
            });
        }

        private async void sendButton_Click(object sender, RoutedEventArgs e)
        {
            Message m = new Message()
            {
                Name = _dataContext.Settings.Name,
                Text = messageInput.Text,
                Direction = Message.DirectionValue.Out
            };

            messageInput.Text = "";

            _dataContext.Messages.Add(m);

            await _dataContext.Communication.SendMessageAsync(m);

            scrollToLast();
        }

        private void messageInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            sendButton.IsEnabled = messageInput.Text.Length > 0;
        }
    }
}