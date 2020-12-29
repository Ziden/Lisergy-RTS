using System;
using System.Collections.Generic;
using System.Text;

namespace LisergyServer.Commands
{
    public abstract class CommandSender
    {
        public abstract void SendMessage(string message);
    }
}
