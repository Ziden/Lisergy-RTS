using Game.Network.ServerPackets;
using Game.Systems.MapPosition;
using Game.Tile;
using System.Linq;
using Game.Engine.Events;
using Game.Engine.ECS;
using Game.Engine.Network;

namespace Game.Systems.Map
{
    public unsafe class MapLogic : BaseEntityLogic<MapPlacementComponent>
    {
        public TileEntity GetPosition()
        {
            return Entity.Components.GetReference<MapReferenceComponent>().Tile;
        }

        public void SetPosition(TileEntity value)
        {
            var component = Entity.Components.GetReference<MapReferenceComponent>();
            var previousTile = component.Tile;
            component.PreviousTile = previousTile;
            component.Tile = value;
            if (previousTile == null || component.Tile == null)
            {
                Entity.DeltaFlags.SetFlag(DeltaFlag.CREATED);
            }
            if (previousTile != null)
            {
                var ev = EventPool<EntityMoveOutEvent>.Get();
                ev.Entity = Entity;
                ev.ToTile = value;
                ev.FromTile = previousTile;
                Entity.Components.CallEvent(ev);
                EventPool<EntityMoveOutEvent>.Return(ev);
            }
            if (value != null)
            {
                var ev = EventPool<EntityMoveInEvent>.Get();
                ev.Entity = Entity;
                ev.ToTile = value;
                ev.FromTile = previousTile;
                Entity.Components.CallEvent(ev);
                EventPool<EntityMoveInEvent>.Return(ev);
            }

            var placement = Entity.Components.GetPointer<MapPlacementComponent>();
            if (component.Tile != null)
            {
                placement->Position = component.Tile.Position;
            }
            else
            {
                placement->Position = default;
                if (previousTile != null)
                {
                    // TODO: Move to system
                    Game.Network.SendToPlayer(new EntityDestroyPacket(Entity), previousTile.PlayersViewing.ToArray());
                }
            }
            Game.Log.Debug($"MapLogic - Set {Entity} position to {component.Tile?.Position}");
        }
    }
}
