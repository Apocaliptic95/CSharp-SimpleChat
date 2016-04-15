using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Network
{
    public partial class Chat : Form
    {
        private Client chat;
        public Chat()
        {
            InitializeComponent();

        }
        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }
            richTextBox1.Text += value+"\n";
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
                chat.Send(textBox1.Text);
                textBox1.Clear();
        }

        private void Chat_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(chat != null)
                chat.Stop();
        }

        private void Chat_Load(object sender, EventArgs e)
        {
            LoginDialog login = new LoginDialog();
            login.ShowDialog();
            chat = new Client(this, login.IP, login.Port, login.ClientName);
            var watek1 = new Thread(chat.Init);
            watek1.Start();
        }
    }
}
