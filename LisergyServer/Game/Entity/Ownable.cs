using Game.DataTypes;
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

    [Serializable]
    public class Ownable : IOwnable
    {
        public Ownable(PlayerEntity owner)
        {
            this.Owner = owner;
        }

        private GameId _ownerId;

        [NonSerialized]
        private PlayerEntity _owner;

        public GameId OwnerID { get => _ownerId; }

        public virtual PlayerEntity Owner
        {
            get => _owner; set
            {
                if (value != null)
                    _ownerId = value.UserID;
                else
                    _ownerId = GameId.ZERO;
                _owner = value;
            }
        }
    }
}
