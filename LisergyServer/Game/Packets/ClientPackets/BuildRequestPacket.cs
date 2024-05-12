using Game.Engine.Network;
using Game.World;
using GameData;
using System;

namespace Game.Network.ClientPackets
{
    [Serializable]
    public class BuildRequestPacket : BasePacket, IClientPacket
    {
        public byte BuilderPartyIndex;
        public BuildingSpecId Building;
        public Location Location;
    }

}
