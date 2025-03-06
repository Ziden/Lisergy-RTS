using Game.Engine.DataTypes;
using Game.Engine.Network;
using Game.World;
using GameData;
using System;

namespace Game.Systems.Building
{
    [Serializable]
    public class BuildCommand : BasePacket, IGameCommand
    {
        public BuildingSpecId Building;
        public Location Location;

        public void Execute(IGame game)
        {
            var tile = game.World.GetTile(Location);
            if(tile.Logic.Building.IsTileFreeForBuilding(Building) != BuildResult.Ok)
            {
                throw new Exception("Bad tile");
            }
        }
    }

}
