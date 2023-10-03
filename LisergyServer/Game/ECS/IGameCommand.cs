using System;
using System.Collections.Generic;
using System.Text;

namespace Game.ECS
{
    public class CommandContext
    {
        public IEntity Entity;
    }

    public interface IGameCommand
    {
        public void Execute(CommandContext context);
    }
}
