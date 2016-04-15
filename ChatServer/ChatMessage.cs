using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class ChatMessage
    {
        private Command _MessageCommand;
        private DateTime _SendDateTime;
        private string _Username;
        private string _Message;

        public Command MessageCommand
        {
            get { return _MessageCommand; }
            set { _MessageCommand = value; }
        }
        public DateTime SendDateTime
        {
            get { return _SendDateTime; }
            set { _SendDateTime = value; }
        }
        public string Username
        {
            get { return _Username; }
            set { _Username = value; }
        }
        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }    
        /// <summary>
        /// Tworzy wiadomosc i pobiera obecną date jako date utworzenia
        /// </summary>
        /// <param name="username"></param>
        /// <param name="message"></param>
        
        public ChatMessage()
        {
            this._Username = "NULL";
            this._Message = "NULL";
            this._SendDateTime = DateTime.Now;
            this._MessageCommand = Command.Message;
        }
        public ChatMessage(string username,string message)
        {
            this._Username = username;
            this._Message = message;
            this._SendDateTime = DateTime.Now;
            this._MessageCommand = Command.Message;
        }
        public ChatMessage(string username,string message,DateTime date)
        {
            this._Username = username;
            this._Message = message;
            this._SendDateTime = date;
            this._MessageCommand = Command.Message;
        }
        public ChatMessage(string username, string message, DateTime date, Command command)
        {
            this._Username = username;
            this._Message = message;
            this._SendDateTime = date;
            this._MessageCommand = command;
        }
        public override string ToString()
        {
            return "[" + _SendDateTime.ToShortTimeString() + "]" + " " + _Username + ": " + _Message;
        }
        public static string SerializeToJSon(ChatMessage chatMessage)
        {
            string output = "";
            JsonSerializer serializer = new JsonSerializer();

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, chatMessage);
                output = sw.ToString();
            }
            return output;
        }
        public static ChatMessage DeserializeFromJson(string jsonString)
        {
            ChatMessage deserializedProduct = JsonConvert.DeserializeObject<ChatMessage>(jsonString);
            return deserializedProduct;
        }
    }
}
