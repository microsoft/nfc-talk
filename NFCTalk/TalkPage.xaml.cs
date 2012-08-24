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

namespace NFCTalk
{
    public partial class TalkPage : PhoneApplicationPage
    {
        NFCTalk.DataContext _dataContext = NFCTalk.DataContext.Singleton();

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
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            foreach (Message m in _dataContext.Messages)
            {
                m.Archived = true;
            }

            base.OnNavigatingFrom(e);
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            _dataContext.Messages.Add(new Message()
            {
                Name = _dataContext.Settings.Name,
                Text = messageInput.Text,
                Direction = Message.DirectionValue.Out
            });

            messageInput.Text = "";

#if DEBUG
            _dataContext.Messages.Add(new Message()
            {
                Name = "Jack Bauer",
                Text = "I already told you, At Connie's, downtown.",
                Direction = Message.DirectionValue.In
            });
#endif

            scrollToLast();
        }

        private void messageInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            sendButton.IsEnabled = messageInput.Text.Length > 0;
        }

        private void talkListBox_Loaded(object sender, RoutedEventArgs e)
        {
            scrollToLast();
        }
    }
}