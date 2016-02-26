using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        Client _client;
        FlowDocument chatsDocs;
        ManualResetEvent clientClosedEvent ;

        public MainWindow()
        {
            InitializeComponent();
            clientClosedEvent = new ManualResetEvent(false);
            chatsDocs = new FlowDocument();
            _client = new Client(clientClosedEvent);
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
                    UpdateChatBox(newMsg + msg.TransmitMsg, msg.ReceivedTime, ClientApplication.HorizontalAlignment.HorizontalLeft);

                    break;
            }
        }

        

        #region Button Events
        private void Btn_Logon_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                _client.ConnectToServer(Convert.ToInt32(txtID.Text), txtServer.Text, 60000, "none");
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
            UpdateChatBox(message + txtChar.Text, DateTime.Now);

            txtChar.Text = "";
        }

        private void Btn_Exit_Clicked(object sender, RoutedEventArgs e)
        {
            clientClosedEvent.Set();
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

        private void UpdateChatBox(string paragraph, DateTime logTime,
            HorizontalAlignment alignment = ClientApplication.HorizontalAlignment.HorizontalRight
            )
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                ChatBox cBox = new ChatBox(paragraph, logTime, alignment);
                chatPanel.Children.Add(cBox.txtBlock);
            }));
        }
        #endregion

        private void Window_Close(object sender, EventArgs e)
        {
            clientClosedEvent.Set();
            _client.Close();
            this.Close();
        }
    }
}
