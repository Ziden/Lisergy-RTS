using Game.ECS;
using Game.Events;
using Game.Events.GameEvents;
using Game.Network;
using Game.Systems.Tile;
using Game.Tile;
using System.Linq;

namespace Game.Systems.FogOfWar
{
    // TODO: Remove
    public class TileVisibilitySystem : GameSystem<TileComponent>
    {
        public TileVisibilitySystem(LisergyGame game) : base(game) { }
        public override void OnEnabled()
        {
            EntityEvents.On<EntityTileVisibilityUpdateEvent>(OnTileExplorationChanged);
        }

        // Move player stuff to player system
        private void OnTileExplorationChanged(IEntity tile, EntityTileVisibilityUpdateEvent ev)
        {
            var tileObj = (TileEntity)tile; // TODO: Remove cast
            var component = tile.Components.GetReference<TileVisibility>();
            var habitants = tile.Components.GetReference<TileHabitants>();
            if (ev.Explored)
            {
                component.EntitiesViewing.Add(ev.Explorer);
                var owner = Players.GetPlayer(ev.Explorer.OwnerID);
                if (component.PlayersViewing.Add(owner))
                {
                    owner.Data.OnceExplored.Add(tileObj);
                    owner.Data.VisibleTiles.Add(tileObj);
                    var e = EventPool<TileVisibilityChangedEvent>.Get();
                    e.Explorer = ev.Explorer;
                    e.Tile = ev.Tile;
                    e.Visible = ev.Explored;
                    ev.Tile.Components.CallEvent(e);
                    EventPool<TileVisibilityChangedEvent>.Return(e);
                    tileObj.SetFlag(DeltaFlag.SELF_REVEALED);
                }
            }
            else
            {
                var owner = Players.GetPlayer(ev.Explorer.OwnerID);
                component.EntitiesViewing.Remove(ev.Explorer);
                if (!component.EntitiesViewing.Any(e => e.OwnerID == ev.Explorer.OwnerID))
                {
                    owner.Data.VisibleTiles.Remove(tileObj);
                    if (component.PlayersViewing.Remove(owner))
                    {
                        var e = EventPool<TileVisibilityChangedEvent>.Get();
                        e.Explorer = ev.Explorer;
                        e.Tile = ev.Tile;
                        e.Visible = ev.Explored;
                        ev.Tile.Components.CallEvent(e);
                        EventPool<TileVisibilityChangedEvent>.Return(e);
                        tileObj.SetFlag(DeltaFlag.SELF_CONCEALED);
                    }
                }
            }
        }
    }
}
