using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatClientWPF
{
   static class CommandHandler
   {
        static public MainWindow ChatWindow;
        internal static void ShowOnlineUsers(ChatMessage message)
        {
            ChatWindow.Dispatcher.Invoke((Action)(() =>
            {
                var users = message.Message.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                ChatWindow.listBoxOnlineUsers.Items.Clear();
                foreach (var user in users)
                {
                    ChatWindow.listBoxOnlineUsers.Items.Add(user);
                }
            }));
        }
    }
}
