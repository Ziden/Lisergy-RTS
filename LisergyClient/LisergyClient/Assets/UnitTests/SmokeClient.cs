using Assets.Code;
using Assets.UnitTests.Behaviours;
using Assets.UnitTests.Stubs;
using Game;
using Game.Network.ClientPackets;
using Game.Systems.Tile;
using Game.Tile;
using Game.World;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.UnitTests
{
    public class SmokeClient
    {
        public PartyBehaviour PartyBehaviour = new PartyBehaviour();
        public LoginBehaviour LoginBehaviour = new LoginBehaviour();

        public EntityType FindFirst<EntityType>() where EntityType : BaseEntity
        {
            return (EntityType)MainBehaviour.LocalPlayer.KnownEntities.Values.FirstOrDefault(e => e.GetType() == typeof(EntityType));
        }

        public TileEntity GetTileInRange(TileEntity source, int range, Direction d)
        {
            while(range > 0)
            {
                source = source.GetNeighbor(d);
                range--;
            }
            return source;
        }
    }
}
