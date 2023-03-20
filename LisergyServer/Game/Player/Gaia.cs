using Game.DataTypes;
using Game.Entity;
using System;

namespace Game.Player
{

    /// <summary>
    /// Gaia is the nature. Mother of nature :D 
    /// </summary>
    [Serializable]
    public class Gaia : PlayerEntity
    {
        public Gaia()
        {
            UserID = GameId.ZERO;
        }

        public static bool IsGaia(GameId userId)
        {
            return userId == GameId.ZERO;
        }

        public static bool Owns(IOwnable ownable)
        {
            return IsGaia(ownable.OwnerID);
        }

        public override bool Online()
        {
            return false; // but its ALWAYS WATCHING YOU
        }

        public override void Send<EventType>(EventType ev)
        {
        }
    }
}
