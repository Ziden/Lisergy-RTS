using Game.GamePlayer;
using GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Comands
{
    /// <summary>
    /// Exposes all player logic objects that can be manipulated inside a command
    /// </summary>
    public interface IPlayerCommandContext
    {
        public IPlayerInventory Inventory { get; }
        public GameSpec GameSpecs { get; }
    }

    public class CommandContext : IPlayerCommandContext
    {
        public IPlayerInventory Inventory { get; set; }
        public GameSpec GameSpecs { get; set; }
    }

}
