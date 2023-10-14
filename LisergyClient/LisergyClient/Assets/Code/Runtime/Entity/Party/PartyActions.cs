using System.Linq;
using Assets.Code.Assets.Code.Runtime;
using Assets.Code.UI;
using Game.Systems.Movement;

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