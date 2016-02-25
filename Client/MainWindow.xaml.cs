using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int _clientID;
        string _clientPassword;
        string _chatContent;
        Client _client;
        FlowDocument chatsDocs;

        public MainWindow()
        {
            InitializeComponent();
            chatsDocs = new FlowDocument();
            _client = new Client();
            _client.ClientEvent += _client_ClientEvent;
        }

        void _client_ClientEvent(object sender, EventType Type, object response)
        {
            switch (Type)
            {
                case EventType.ClientEvent:
                    UpdateStatus(response.ToString());
                    break;
                case EventType.ServerEvent:
                    Paragraph paragraph = new Paragraph();

                    paragraph.Foreground = Brushes.Blue;

                    Message msg = (Message)response;

                    paragraph.Inlines.Add(msg.TransmitMsg);

                    string newMsg = "Terminal " + msg.MsgFrom.ToString() + ": ";
                    UpdateChatBox(newMsg + msg.TransmitMsg);

                    break;
            }
        }

        

        #region Button Events
        private void Btn_Logon_Clicked(object sender, RoutedEventArgs e)
        {

            try
            {
                _client.ConnectToServer(Convert.ToInt32(txtID.Text), "127.0.0.1", 60000, "none");
            }
            catch (System.Exception ex)
            {
            }
        }

        private void Btn_Send_Clicked(object sender, RoutedEventArgs e)
        {
            _client.SendMessage(Convert.ToInt32(txtTo.Text), txtChar.Text);
          //  Paragraph paragraph = new Paragraph();
           // paragraph.Foreground = Brushes.Black;

          //  paragraph.Inlines.Add(txtChar.Text.ToString());
            string message = "Terminal " + _client.ClientID +": ";
            UpdateChatBox(message + txtChar.Text);
        }

        private void Btn_Exit_Clicked(object sender, RoutedEventArgs e)
        {
            _client.Close();
        }

        void UpdateStatus(string message)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                lblStatus.Foreground = Brushes.Black;
                lblStatus.Content = message;
            }));
        }

        //private void UpdateChatBox(Paragraph paragraph)
        //{
        //    this.Dispatcher.BeginInvoke((Action)(() =>
        //    {
        //        // paragraph.Inlines.Add(new LineBreak());

        //        chatsDocs.Blocks.Add(paragraph);

        //        txtChatsBox.Document = chatsDocs;
        //    }));
        //}

        private void UpdateChatBox(string paragraph)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                txtChatsBox.Text += paragraph + "\n";
            }));
        }
        #endregion

        private void Window_Close(object sender, EventArgs e)
        {
            _client.Close();
            this.Close();
        }
    }
}
