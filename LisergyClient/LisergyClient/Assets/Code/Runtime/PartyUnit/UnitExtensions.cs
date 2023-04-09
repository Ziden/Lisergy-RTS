using Game.Battler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Assets.Code.Runtime.PartyUnit
{
    public static class UnitExtensions
    {
        /// <summary>
        /// 1 = max hp
        /// 0 = empty
        /// </summary>
        public static float GetHpRatio(this Unit u)
        {
            return u.HP / (float)u.MaxHP;
        }
    }
}
