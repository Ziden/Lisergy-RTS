using Game.Engine.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Systems.Player
{
    /// <summary>
    /// Represents basic data of a player profile that will be shared with the client
    /// </summary>
    [Serializable]
    public class PlayerProfile
    {
        private GameId _playerId;
        public string PlayerName;

        public PlayerProfile(in GameId id)
        {
            _playerId = id;
        }

        public ref GameId PlayerId => ref _playerId;

        public override string ToString()
        {
            return $"<Profile PlayerId={_playerId} Name={PlayerName}/>";
        }
    }
}
