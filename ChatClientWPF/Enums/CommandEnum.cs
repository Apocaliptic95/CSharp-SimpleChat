using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClientWPF
{
    enum Command
    {
        Message = 0x001,
        File = 0x002,
        ClientLogOff = 0x003,
        OnlineUsers = 0x004,
        Login = 0x005
    }
}
