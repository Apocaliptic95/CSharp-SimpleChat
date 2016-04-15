namespace ChatServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            Server serv = new Server();
            serv.Start();
        }
    }
}