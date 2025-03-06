using Game.Engine.ECLS;
using GameData;
using System;

namespace Game.Systems.Building
{
    [Serializable]
    [SyncedComponent]
    /// <summary>
    /// Represents a building owned by a player.
    /// </summary>
    /// <remarks>
    /// This component is used to associate a building with a player in the game.
    /// It contains the specification ID of the building, which can be used to 
    /// retrieve detailed information about the building's properties and behavior.
    /// </remarks>
    public class PlayerBuildingComponent : IComponent
    {
        public BuildingSpecId SpecId;
    }
}
