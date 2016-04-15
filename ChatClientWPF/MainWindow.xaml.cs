using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Drawing;
using System.Windows.Forms;

namespace ChatClientWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public NotifyIcon nIcon = new NotifyIcon();
        private ConfigFile config = new ConfigFile();
        public MainWindow()
        {
            InitializeComponent();
            CommandHandler.ChatWindow = this;
            textBoxNick.Text = config.Search("nick").Value;
            textBoxIP.Text = config.Search("ip").Value;
            textBoxPort.Text = config.Search("port").Value;
        }

        delegate void ParametrizedMethodInvoker(string value);
        public void AddTextToChat(string value)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new ParametrizedMethodInvoker(AddTextToChat), value);
                return;
            }
            RTextBoxChat.AppendText(value+"\n");
            RTextBoxChat.ScrollToEnd();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            //AddTextToChat(textBoxMessage.Text);
            Client.Send(textBoxNick.Text, textBoxMessage.Text);
            textBoxMessage.Clear();
        }
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            textBoxIP.Text = textBoxIP.Text.TrimEnd();
            textBoxPort.Text = textBoxPort.Text.TrimEnd();
            textBoxNick.Text = textBoxNick.Text.TrimEnd();

            if (btnConnect.Content.ToString() == "Connect")
            {
                AddTextToChat("Connecting to the server...");
                try
                {
                    Client.Connect(textBoxIP.Text, Convert.ToInt32(textBoxPort.Text), this);
                    btnConnect.Content = "Disconnect";
                    btnSend.IsEnabled = true;

                    textBoxIP.IsEnabled = false;
                    textBoxPort.IsEnabled = false;
                    textBoxNick.IsEnabled = false;
                    config.Change("nick",textBoxNick.Text);
                    config.Change("ip", textBoxIP.Text);
                    config.Change("port", textBoxPort.Text);
                    config.Save();
                }
                catch (Exception ex)
                {
                    AddTextToChat(ex.Message);
                }
            }
            else
            {
                btnSend.IsEnabled = false;
                btnConnect.Content = "Connect";
                Client.Disconnect();

                textBoxIP.IsEnabled = true;
                textBoxPort.IsEnabled = true;
                textBoxNick.IsEnabled = true;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if(Client.ClientTCP != null && Client.ClientTCP.Connected) Client.Disconnect();
            base.OnClosing(e);
        }
        internal void ShowNotifyIcon(int msTimeout,string title, string msg)
        {
            this.nIcon.Visible = true;
            this.nIcon.Icon = ChatClientWPF.Properties.Resources.NotificationIcon;
            this.nIcon.ShowBalloonTip(msTimeout, title, msg, ToolTipIcon.Info);       
        }

        void nIcon_BalloonTipClosed(object sender, EventArgs e)
        {
            nIcon.Visible = false;
        }
    }
}
