using System;
using System.Collections.Generic;
using Npgsql;

namespace ChatServer
{
    /*
    
    Host
        ec2-54-195-252-202.eu-west-1.compute.amazonaws.com
    Database
        dc7ao032k002lj
    User
        nnyvsryrrrfxfz
    Port
        5432
    Password
        BpH0jQZG8h0RjMGDxk8Ipf1wwK 
    Psql
        heroku pg:psql --app heroku-postgres-6cda2867 HEROKU_POSTGRESQL_CYAN
    URL
        postgres://nnyvsryrrrfxfz:BpH0jQZG8h0RjMGDxk8Ipf1wwK@ec2-54-195-252-202.eu-west-1.compute.amazonaws.com:5432/dc7ao032k002lj 
    
        
       EXAMPLE IMPLEMENTATION:
            var baza = new DataBase();
            var message = baza.GetLastMessages();
            baza.InsertMessage(new Message("lisek","ciepla"));
            foreach (var element in message)
            {
                Console.WriteLine("[{0}] {1}: {2}",element.GetTime(), element.GetNick(), element.GetMessage());
            } 
        
        
        */

    class DataBase
    {
        //private string _db_host = "ec2-54-195-252-202.eu-west-1.compute.amazonaws.com";
        //private string _db_login = "nnyvsryrrrfxfz";
        //private string _db_password = "BpH0jQZG8h0RjMGDxk8Ipf1wwK";
        //private string _db_database = "dc7ao032k002lj";
        //private string _db_host = "127.0.0.1";
        //private string _db_login = "postgres";
        //private string _db_password = "StefanKrul321";
        //private string _db_database = "chat";
        private NpgsqlConnection _conn;


        public DataBase(string host, string port, string login, string password, string database)
        {

            try
            {
                Console.WriteLine("Connecting to the database.");
                _conn =
                    new NpgsqlConnection("Server=" + host + ";Port="+port+";Username=" + login + ";Password=" +
                                         password +
                                         ";Database=" + database + "");
                _conn.Open();
                Console.WriteLine("Success.");
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine(e.Message);
                Console.Read();
            }
            catch (System.TimeoutException e)
            {
                Console.WriteLine(e.Message);
                Console.Read();
            }
        }

        public void InsertMessage(ChatMessage input)
        {
            try
            {
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = _conn;
                    cmd.CommandText = "Insert Into messages (nick,message,date) VALUES " +
                                      "('" + input.Username + "','" + input.Message + "', 'NOW()');";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine(e.Message);
                Console.Read();
            }
        }

        public List<ChatMessage> GetLastMessages()
        {
            List<ChatMessage> lista = new List<ChatMessage>();
            try
            {
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = _conn;

                    cmd.CommandText = "SELECT nick, message, date FROM messages limit 200";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new ChatMessage(reader.GetString(0), reader.GetString(1), (DateTime)reader.GetTimeStamp(2)));

                        }
                    }
                }
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine(e.Message);
                Console.Read();
            }
            return lista;
        }

    }
}
