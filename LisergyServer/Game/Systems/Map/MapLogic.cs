using Game.ECS;
using Game.Events.GameEvents;
using Game.Network.ServerPackets;
using Game.Network;
using Game.Systems.FogOfWar;
using Game.Systems.MapPosition;
using Game.Tile;

namespace Game.Systems.Map
{
    public class MapLogic : BaseComponentLogic<MapPositionComponent>
    {
        public TileEntity GetPosition()
        {
            return Entity.Get<MapReferenceComponent>().Tile;
        }

        public void SetPosition(TileEntity value)
        {
            var component = Entity.Get<MapReferenceComponent>();
            component.PreviousTile = component.Tile;
            component.Tile = value;

            if (component.PreviousTile == null || component.Tile == null)
            {
                Entity.DeltaFlags.SetFlag(DeltaFlag.EXISTENCE);
            }
            else if (component.PreviousTile != component.Tile)
            {
                Entity.DeltaFlags.SetFlag(DeltaFlag.POSITION);
            }

            if (component.PreviousTile != null)
            {
                var moveOut = new EntityMoveOutEvent()
                {
                    Entity = Entity,
                    ToTile = value,
                    FromTile = component.PreviousTile
                };
                // TODO NOT CALL EVENTS ON TILE 
                component.PreviousTile.Components.CallEvent(moveOut);
                Entity.Components.CallEvent(moveOut);
            }
            if (value != null)
            {
                var moveIn = new EntityMoveInEvent()
                {
                    Entity = Entity,
                    ToTile = component.Tile,
                    FromTile = component.PreviousTile
                };
                value.Components.CallEvent(moveIn);
                Entity.Components.CallEvent(moveIn);
            }

            var location = Entity.Get<MapPositionComponent>();
            if (component.Tile != null)
            {
                location.X = component.Tile.X;
                location.Y = component.Tile.Y;
            }
            else
            {
                location.X = 0;
                location.Y = 0;
                if (component.PreviousTile != null)
                {
                    foreach (var viewer in component.PreviousTile.Components.Get<TileVisibility>().PlayersViewing)
                        viewer.Send(new EntityDestroyPacket(Entity));
                }
            }
            Log.Info($"Moved {Entity} to {component.Tile}");
        }
    }
}
