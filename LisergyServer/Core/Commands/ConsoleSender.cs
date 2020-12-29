using Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace LisergyServer.Commands
{
    public class ConsoleSender : CommandSender
    {
        public override void SendMessage(string message)
        {
            Log.Info(message);
        }
    }
}
