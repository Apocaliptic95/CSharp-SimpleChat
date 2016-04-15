using System;
using System.IO;
using System.Threading;
using System.Net.Sockets;

namespace ChatServer
{
    class Client
    {
        public delegate void MessageCallback(Client sender, String message);
        public event MessageCallback MessageReceivedEvent;

        public delegate void ConnectCallback(Client sender);
        public event ConnectCallback ClientConnectedEvent;
        public event ConnectCallback ClientDisconnectedEvent;
        
        private ManualResetEvent _doneEvent;
        private Socket _socket;
        private NetworkStream _clientStream;
        private StreamReader _clientStreamReader;
        private StreamWriter _clientStreamWriter;
        private String _userName;

        public String getUserName()
        {
            return _userName;
        }

        public Client(Socket socket, ManualResetEvent doneEvent)
        {
            _socket = socket;
            _clientStream = new NetworkStream(_socket);
            _clientStreamReader = new StreamReader(_clientStream);
            _clientStreamWriter = new StreamWriter(_clientStream);
            _doneEvent = doneEvent;
        }

        public void Send(String message)
        {
            try
            {
                _clientStreamWriter.WriteLine(message);
                _clientStreamWriter.Flush();
            }
            catch(IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void Receive()
        {
            while(isConnected())
            {
                try
                {
                    String wiadomosc = _clientStreamReader.ReadLine();
                    if (wiadomosc != null && wiadomosc.Length > 0)
                    {
                        var message = wiadomosc.Split(' ');
                        if (message[0] == Command.Login.ToString())
                        {
                            _userName = message[1];
                            ClientConnectedEvent(this);
                        }
                        else
                        {
                            MessageReceivedEvent(this, wiadomosc);
                        }
                    }
                }
                catch(IOException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void ThreadPoolCallback(Object threadContext)
        {
            int threadIndex = (int)threadContext;
            Receive();
            ClientDisconnectedEvent(this);
            _doneEvent.Set();
        }

        private bool isConnected()
        {
            try
            {
                return !(_socket.Poll(1, SelectMode.SelectRead) && _socket.Available == 0);
            }
            catch (SocketException) 
            { 
                return false; 
            }
        }
    }
}
