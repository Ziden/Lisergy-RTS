using System;
using System.Collections.Generic;
using System.Text;

namespace LisergyServer.Commands
{
    public abstract class Command
    {
        public abstract string GetCommand();

        public abstract void Execute(CommandSender sender, CommandArgs args);

        public int GetPermissionLevel()
        {
            return 0;
        }

    }
}
