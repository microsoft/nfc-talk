using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NFCTalk
{
    /// <summary>
    /// TalkPage displays the currently active chat session and also
    /// grayed out messages from previous chat sessions.
    /// </summary>
    public partial class TalkPage : PhoneApplicationPage
    {
        private NFCTalk.DataContext _dataContext = NFCTalk.DataContext.Singleton;

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

        /// <summary>
        /// Chat message list is scrolled to the last message sent or received and listening to
        /// incoming messages is started.
        /// </summary>
        /// <param name="e"></param>
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

        /// <summary>
        /// Leaving TalkPage causes the current chat session to be disconnected and all
        /// stored messages to be marked as archived.
        /// </summary>
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

        /// <summary>
        /// Event handler to be executed when messages change.
        /// 
        /// TalkPage message list is scrolled to the last message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessagesChanged(object sender, EventArgs e)
        {
            scrollToLast();
        }

        /// <summary>
        /// Event handler to be executed when a new inbound message has been received.
        /// 
        /// Message is stored to the DataContext.Messages.
        /// </summary>
        /// <param name="m"></param>
        private void MessageReceived(Message m)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                _dataContext.Messages.Add(m);
            });
        }

        /// <summary>
        /// Event handler to be executed when connection is interrupted.
        /// 
        /// Chat session is disconnected and application navigates back to the MainPage.
        /// </summary>
        private void ConnectionInterrupted()
        {
            _dataContext.Communication.Disconnect();

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.GoBack();
            });
        }

        /// <summary>
        /// Event handler to be executed when send button is clicked.
        /// 
        /// New outbound message is constructed using the configured chat name and
        /// chat message from the message input field. Message is send to the other device.
        /// </summary>
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

        /// <summary>
        /// Event handler to be executed when message input field content changes.
        /// 
        /// Send button is enabled if message exists, otherwise send button is disabled.
        /// </summary>
        private void messageInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            sendButton.IsEnabled = messageInput.Text.Length > 0;
        }
    }
}