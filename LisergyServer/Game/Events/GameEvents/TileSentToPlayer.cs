using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.GameEvents
{
    public class TileSentToPlayerEvent : GameEvent
    {
        public Tile Tile;
        public PlayerEntity Player;

        public TileSentToPlayerEvent(Tile tile, PlayerEntity player)
        {
            Tile = tile;
            Player = player;
        }
    }
}
