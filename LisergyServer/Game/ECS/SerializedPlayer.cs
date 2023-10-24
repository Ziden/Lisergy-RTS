using Game.DataTypes;
using Game.Systems.Player;
using Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.ECS
{
    [Serializable]
    public class SerializedPlayer
    {
        public GameId PlayerId;
        public PlayerData Data;
        public List<Position> SeenTiles;

        public SerializedPlayer() { }

        public SerializedPlayer(PlayerEntity player)
        {
            PlayerId = player.EntityId;
            Data = player.Data;
            SeenTiles = player.VisibilityReferences.OnceExplored.ToList();
        }
    }
}
