using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    [Serializable]
    public class Ownable
    {
        public Ownable(PlayerEntity owner)
        {
            this.Owner = owner;
        }

        protected string _ownerId;

        [NonSerialized]
        private PlayerEntity _owner;

        public string OwnerID { get => _ownerId; }

        public virtual PlayerEntity Owner
        {
            get => _owner; set
            {
                if (value != null)
                    _ownerId = value.UserID;
                else
                    _ownerId = null;
                _owner = value;
            }
        }

     
    }
}
