using Game.Engine.DataTypes;
using Game.Systems.Player;
using Game.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Engine.ECLS
{
    [Serializable]
    public class SerializedPlayer
    {
        public GameId PlayerId;
        public PlayerDataComponent Data;
        public List<Location> SeenTiles;

        public SerializedPlayer() { }

        public SerializedPlayer(PlayerModel player)
        {
            PlayerId = player.EntityId;
            Data = player.Components.Get<PlayerDataComponent>();
            SeenTiles = player.Components.Get<PlayerVisibilityComponent>().OnceExplored.ToList();
        }
    }
}
