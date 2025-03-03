using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using System;

namespace Game.Systems.Player
{
    /// <summary>
    /// Represents basic data of a player profile that will be shared with the client
    /// </summary>
    [Serializable]
    public class PlayerProfileComponent : IComponent
    {
        public GameId PlayerId;
        public string Name;

        public PlayerProfileComponent(in GameId id)
        {
            PlayerId = id;
        }

        public override string ToString()
        {
            return $"<Profile PlayerId={PlayerId} Name={Name}/>";
        }
    }
}
