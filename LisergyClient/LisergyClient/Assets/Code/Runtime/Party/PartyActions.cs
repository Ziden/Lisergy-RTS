using System.Collections.Generic;
using System.Linq;
using Assets.Code.Assets.Code.Runtime;
using Assets.Code.Assets.Code.Runtime.Movement;
using Assets.Code.Code.Utils;
using Assets.Code.Entity;
using Assets.Code.UI;
using Assets.Code.Views;
using DG.Tweening;
using Game.DataTypes;
using Game.Events.GameEvents;
using Game.Network.ClientPackets;
using Game.Systems.Movement;
using Game.Tile;
using Game.World;
using UnityEngine;

namespace Assets.Code.World
{
    public class PartyActions
    {
        public static void PerformActionWithSelectedParty(EntityAction action)
        {
            var tile = ClientState.SelectedTile;
            var party = ClientState.SelectedParty;
           
            var map = tile.Chunk.Map;
            var path = map.FindPath(party.Tile, tile);
            var tilePath = path.Select(node => map.GetTile(node.X, node.Y)).ToList();
            ClientEvents.StartMovementRequest(party, tilePath);
            var intent = action == EntityAction.ATTACK ? CourseIntent.Offensive : CourseIntent.Defensive;
            /*
            MainBehaviour.Networking.Send(new MoveRequestPacket()
            {
                PartyIndex = party.PartyIndex,
                Path = path.Select(p => new Position(p.X, p.Y)).ToList(),
                Intent = intent
            });
            */
        }

    }
}