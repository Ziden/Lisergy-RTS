using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Comands
{
    /// <summary>
    /// Game commands are run on the game and on the WebPlayerLogic server in a deterministic way
    /// Commands alter player data (not world data)
    /// </summary>
    public interface IGameCommand
    {
    }
}
