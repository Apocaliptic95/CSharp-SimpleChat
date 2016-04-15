using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ChatClientWPF;


namespace ChatServer
{

    class Server
    {
        private TcpListener _listener;
        private List<ManualResetEvent> _doneEvents;
        private List<Client> _clientsList;
        private ConfigFile config = new ConfigFile();
        private DataBase sql;
        private String _serverName = "Server";

        public Server(int Port = 666)
        {
            try
            {
                sql  = new DataBase(config.Search("host").Value, config.Search("port").Value, config.Search("login").Value, config.Search("password").Value, config.Search("database").Value);
                _listener = new TcpListener(IPAddress.Any, Port);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            _doneEvents = new List<ManualResetEvent>();
            _clientsList = new List<Client>();
        }

        public Server(IPAddress IP, int Port = 666)
        {
            try
            {
                _listener = new TcpListener(IPAddress.Any, Port);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            _doneEvents = new List<ManualResetEvent>();
            _clientsList = new List<Client>();
        }

        public void Start()
        {
            Console.WriteLine(_serverName+" has Started.");
            _listener.Start();

            int counter = 0;
            while (true)
            {
                _doneEvents.Add(new ManualResetEvent(false));
                Client tC = new Client(_listener.AcceptSocket(), _doneEvents.ElementAt(counter));
                tC.MessageReceivedEvent += ClientMessageReceived;
                tC.ClientConnectedEvent += ClientConnected;
                tC.ClientDisconnectedEvent += ClientDisconnected;
                _clientsList.Add(tC);
                ThreadPool.QueueUserWorkItem(tC.ThreadPoolCallback, counter);
                counter++;
            }
            WaitHandle.WaitAll(_doneEvents.ToArray());
            Console.WriteLine("Done.");
        }

        public void SendToAll(String message)
        {
            foreach(Client client in _clientsList)
            {
                client.Send(message);
            }
        }

        private void ClientMessageReceived(object sender, String message)
        {
            var wiadomosc = ChatMessage.DeserializeFromJson(message);
            if (wiadomosc.Username != _serverName)
                    sql.InsertMessage(wiadomosc);
            SendToAll(message);
        }

        private void ClientConnected(Client sender)
        {
            Console.WriteLine(sender.getUserName() + " connected.");
            ChatMessage cm = new ChatMessage(_serverName, sender.getUserName() + " joined to the chat.");
            SendToAll(ChatMessage.SerializeToJSon(cm));
            foreach (var element in sql.GetLastMessages())
            {
                var wiadomosc = ChatMessage.SerializeToJSon(element);
                sender.Send(wiadomosc);
            }
            SendConnectedClientsList();
        }

        private void ClientDisconnected(Client sender)
        {
            _clientsList.Remove(sender);
            Console.WriteLine(sender.getUserName() + " disconnected.");
            ChatMessage cm = new ChatMessage(_serverName, sender.getUserName() + " disconnected from the chat.");
            SendToAll(ChatMessage.SerializeToJSon(cm));
            SendConnectedClientsList();
        }

        private void SendConnectedClientsList()
        {
            string clients = "";
            foreach(Client c in _clientsList)
            {
                clients += c.getUserName()+",";
            }
            if(clients.Length != 0)
                clients = clients.Remove(clients.Length - 1);
            ChatMessage cm = new ChatMessage(_serverName, clients, DateTime.Now, Command.OnlineUsers);
            SendToAll(ChatMessage.SerializeToJSon(cm));
        }
    }
}