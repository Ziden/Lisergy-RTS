using Game.Engine.Network;
using Game.World;
using GameData;
using System;

namespace Game.Systems.Building
{
    [Serializable]
    public class BuildCommand : BasePacket, IGameCommand
    {
        public byte BuilderPartyIndex;
        public BuildingSpecId Building;
        public Location Location;

        public void Execute(IGame game)
        {
            throw new NotImplementedException();
        }
    }

}
