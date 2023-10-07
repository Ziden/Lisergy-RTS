using Game.ECS;
using Game.Events.GameEvents;
using Game.Network.ServerPackets;
using Game.Network;
using Game.Systems.FogOfWar;
using Game.Systems.MapPosition;
using Game.Tile;
using System.Linq;

namespace Game.Systems.Map
{
    public class MapLogic : BaseEntityLogic<MapPlacementComponent>
    {
        public TileEntity GetPosition()
        {
            return Entity.Get<MapReferenceComponent>().Tile;
        }

        public void SetPosition(TileEntity value)
        {
            var component = Entity.Get<MapReferenceComponent>();
            var previousTile = component.Tile;
            component.PreviousTile = previousTile;
            component.Tile = value;

            if (previousTile == null || component.Tile == null)
            {
                Entity.DeltaFlags.SetFlag(DeltaFlag.EXISTENCE);
            }
            else if (previousTile != component.Tile)
            {
                Entity.DeltaFlags.SetFlag(DeltaFlag.COMPONENTS);
            }
            if (previousTile != null)
            {
                var moveOut = new EntityMoveOutEvent()
                {
                    Entity = Entity,
                    ToTile = value,
                    FromTile = previousTile
                };
                // TODO NOT CALL EVENTS ON TILE 
                previousTile.Components.CallEvent(moveOut);
                Entity.Components.CallEvent(moveOut);
            }
            if (value != null)
            {
                var moveIn = new EntityMoveInEvent()
                {
                    Entity = Entity,
                    ToTile = component.Tile,
                    FromTile = previousTile
                };
                value.Components.CallEvent(moveIn);
                Entity.Components.CallEvent(moveIn);
            }

            var placement = Entity.Get<MapPlacementComponent>();
            if (component.Tile != null)
            {
                placement.Position = component.Tile.Position;
            }
            else
            {
                placement.Position = default;
                if (previousTile != null)
                {
                    // TODO: Move to system
                    Game.Network.SendToPlayer(new EntityDestroyPacket(Entity), previousTile.Components.Get<TileVisibility>().PlayersViewing.ToArray());
                }
            }
            Log.Info($"Moved {Entity} to {component.Tile}");
        }
    }
}
