using Game.ECS;
using Game.Systems.Battler;
using System;

namespace Game.Systems.Party
{
    [SyncedComponent]
    [Serializable]
    public class PartyComponent : IComponent
    {
        /// <summary>
        /// Party index on player party array
        /// </summary>
        public byte PartyIndex;

        public override string ToString()
        {
            return $"<PartyComponent Index={PartyIndex}>";
        }
    }
}
