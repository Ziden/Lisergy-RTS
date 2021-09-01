using Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace LisergyServer.Commands
{
    public abstract class Command
    {
        protected BlockchainGame Game;

        public abstract string GetCommand();

        public abstract string Description();

        public abstract void Execute(CommandSender sender, CommandArgs args);

        public Command(BlockchainGame game)
        {
            this.Game = game;
        }

        public int GetPermissionLevel()
        {
            return 0;
        }

    }
}
