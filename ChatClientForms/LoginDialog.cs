using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Network
{
    public partial class LoginDialog : Form
    {
        public IPAddress IP
        {
            get;
            set;
        }
        public String ClientName
        {
            get;
            set;
        }
        public int Port
        {
            get;
            set;
        }
        public LoginDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string pattern = @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";
            if (ip.Text.Length > 0 
                && nick.Text.Length > 3 
                && port.Text.Length > 0 
                && Regex.IsMatch(ip.Text,pattern))
            {
                IP = IPAddress.Parse(ip.Text);
                ClientName = nick.Text;
                Port = Convert.ToInt32(port.Text);
                this.Close();
            }
        }

        private void ip_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
