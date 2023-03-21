using Assets.Code;
using Assets.UnitTests.Behaviours;
using Assets.UnitTests.Stubs;
using Game.Network.ClientPackets;
using Game.Party;
using Game.Tile;
using System.Collections;
using UnityEngine;

namespace Assets.UnitTests
{
    public class SmokeClient
    {

        public LoginBehaviour LoginBehaviour = new LoginBehaviour();

        public void SelectParty(PartyEntity p)
        {
            ClientEvents.SelectParty(p);
        }

        public void ClickTile(TileEntity tile)
        {
            ClientEvents.ClickTile(tile);
        }
    }
}
