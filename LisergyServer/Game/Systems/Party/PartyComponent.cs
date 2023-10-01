using Game.ECS;
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
    }
}
