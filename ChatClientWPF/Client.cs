using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatClientWPF
{
    class Client
    {
        private static MainWindow Parent;
        private static Thread _ListeningThread;
        private static StreamReader _ReadingStream;
        private static StreamWriter _WritingStream;
        private static TcpClient _ClientTCP;
        private static string _UserNickname;
        static Stream BeepSound = Properties.Resources.beep;
        static SoundPlayer NewMessagePlayer = new SoundPlayer(BeepSound);
        

        public string UserNickname
        {
            get { return _UserNickname; }
            set { _UserNickname = value; }
        }
        
        public static StreamReader ReadingStream
        {
            get { return Client._ReadingStream; }
        }
        public static StreamWriter WritingStream
        {
            get { return Client._WritingStream; }
        }
        public static TcpClient ClientTCP
        {
            get { return _ClientTCP; }
        }
        public static Thread ListeningThread
        {
            get { return _ListeningThread; }
        }
        
        public static void ListenToTheServer()
        {
            string incomingMsg;
            while (_ClientTCP.Connected)
            {
                try
                {
                    incomingMsg = _ReadingStream.ReadLine();
                    var oMessage = ChatMessage.DeserializeFromJson(incomingMsg);
                    if (incomingMsg.Length > 0 && oMessage.MessageCommand == Command.Message)
                    {
                        Parent.AddTextToChat(oMessage.ToString());
                        if (oMessage.Username != _UserNickname) PlayChatSound();
                    }
                    else if (oMessage.MessageCommand == Command.OnlineUsers)
                        CommandHandler.ShowOnlineUsers(oMessage);                                       
                }
                catch (Exception ex)
                {
                    Parent.AddTextToChat(ex.Message);
                    break;
                }
            }
            Parent.AddTextToChat("Disconnected!");

            //Kontrole nad UI ma główny wątek i tak trzeba się do niego odwoływać z innych wątków
            if(_ListeningThread.IsAlive)
            Parent.Dispatcher.Invoke((Action)(() =>
            {
                Parent.btnConnect.IsEnabled = true;
                Parent.btnSend.IsEnabled = false;
            }));
        }
        public static void PlayChatSound()
        {
            Parent.Dispatcher.Invoke((Action)(() =>
            {               
                if (Parent.checkBoxSound.IsChecked.Value)
                    NewMessagePlayer.Play();
            }));
        }
        public static void Connect(string ip, int port,MainWindow parent)
        {
            Parent = parent;
            _ClientTCP = new TcpClient();
            _ClientTCP.Connect(ip, port);
            var netStream = _ClientTCP.GetStream();
            _ReadingStream = new StreamReader(netStream);
            _WritingStream = new StreamWriter(netStream);
            _UserNickname = Parent.textBoxNick.Text;
            try
                {                
                    string logInInfo = Command.Login.ToString() + " " + _UserNickname;
                    _WritingStream.WriteLine(logInInfo);
                    _WritingStream.Flush();
                }
            catch (Exception ex)
                {
                    Parent.AddTextToChat(ex.Message);
                }

            _ListeningThread = new Thread(ListenToTheServer);
            _ListeningThread.Start();

 
        }
        public static void Disconnect()
        {
            if (_ClientTCP.Connected)
            {
                _ListeningThread.Abort();
                _ClientTCP.GetStream().Close();
                _ClientTCP.Close();
                Parent.AddTextToChat("Disconnected");
                Parent.listBoxOnlineUsers.Items.Clear();
            }
        }
        public static void Send(string username, string message)
        {
            var oMessage = new ChatMessage(username, message);
            _WritingStream.WriteLine(ChatMessage.SerializeToJSon(oMessage));
            _WritingStream.Flush();
        }
    }
}
