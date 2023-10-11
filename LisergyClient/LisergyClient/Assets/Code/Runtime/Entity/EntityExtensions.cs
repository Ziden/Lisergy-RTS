using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Entity
{
    public static class EntityExtensions
    {
        public static bool IsMine(this BaseEntity entity) => entity.OwnerID == MainBehaviour.LocalPlayer.OwnerID;
    }
}
