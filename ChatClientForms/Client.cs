using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Network
{
    class Client
    {
        public TcpClient client;
        private NetworkStream netStream;
        private StreamReader innyStream;
        private StreamWriter Wyslanie;
        private Chat formularz;
        public Client(Chat form, IPAddress IP, int PORT, string name)
        {
            formularz = form;
            client = new TcpClient();
            client.Connect(IP, PORT);
            netStream = client.GetStream();
            innyStream = new StreamReader(netStream);
            Wyslanie = new StreamWriter(netStream);
            Wyslanie.WriteLine(Command.Login.ToString()+" "+name);
            Wyslanie.Flush();
        }

        public void Stop()
        {
            client.GetStream().Close();
            client.Close();
        }           

        public void Send(string message)
        {
                        Wyslanie.WriteLine(message);
                        Wyslanie.Flush();
        }
        public void Init()
        {
            string wiadomosc;
            try
            {
                while (client.Connected)
                {
                    wiadomosc = innyStream.ReadLine();
                    if (wiadomosc.Length > 0)
                        formularz.AppendTextBox("Obcy: " + wiadomosc);
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                client.Close();
            }
        }
    }
}
