using Game.ECS;
using System;

namespace Game.Systems.Party
{
    [SyncedComponent]
    [Serializable]
    public struct PartyComponent : IComponent
    {
        /// <summary>
        /// Party index on player party array
        /// </summary>
        public byte PartyIndex;

        public override string ToString() => $"<PartyComponent Index={PartyIndex}>";
    }
}
