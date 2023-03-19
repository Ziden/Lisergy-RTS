using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Events.GameEvents
{
    /// <summary>
    /// Event whenever we need to send tile information to the client
    /// </summary>
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
