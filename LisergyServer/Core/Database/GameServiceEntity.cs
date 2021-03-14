using LisergyServer.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LisergyServer.Database
{
    public class GameServiceEntity
    {
        public string IP;
        public int Port;
        public ServerType Type;

        public override string ToString()
        {
            return $"<Service {Type} @{IP}-{Port}>";
        }
    }
}
