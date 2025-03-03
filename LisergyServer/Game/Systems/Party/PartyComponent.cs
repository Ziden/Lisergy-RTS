using Game.Engine.ECLS;
using System;

namespace Game.Systems.Party
{
    /// <summary>
    /// Refers to an entity that stays in the party slots of its owner
    /// </summary>
    [SyncedComponent]
    [Serializable]
    public class PartyComponent : IComponent
    {
        /// <summary>
        /// Party index on player party array
        /// </summary>
        public byte PartyIndex;

        public override string ToString() => $"<PartyComponent Index={PartyIndex}>";
    }
}
