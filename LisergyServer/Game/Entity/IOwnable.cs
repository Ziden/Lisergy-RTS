using Game.DataTypes;
using Game.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{

    public interface IOwnable
    {
        PlayerEntity Owner { get; }

        GameId OwnerID { get; }
    }
}
