using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Entity
{

    // Gaia is nature
    [Serializable]
    public class Gaia : PlayerEntity
    {

        public Gaia()
        {
            this.UserID = Guid.Empty;
        }

        public static bool IsGaia(GameId userId)
        {
            return userId == Guid.Empty;
        }

        public override bool Online()
        {
            return false;
        }

        public override void Send<EventType>(EventType ev)
        {
        }
    }
}
