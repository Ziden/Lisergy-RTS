using LisergyServer.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LisergyServer.Auth
{
    public class Account
    {
        public Guid ID;
        public string Login;
        public string Password;

        public ServerPlayer Player;
    }
}
