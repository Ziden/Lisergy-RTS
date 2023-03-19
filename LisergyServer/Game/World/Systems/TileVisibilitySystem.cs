
using Game.ECS;
using Game.Events.GameEvents;
using Game.World.Components;
using System.Linq;

namespace Game.World.Systems
{
    public class TileVisibilitySystem : GameSystem<TileVisibility, Tile>
    {
        internal override void OnComponentAdded(Tile owner, TileVisibility component, EntitySharedEventBus<Tile> events)
        {

            events.RegisterComponentEvent<TileExplorationStateChanged, TileVisibility>(OnTileExplorationChanged);
        }

        private static void OnTileExplorationChanged(Tile tile, TileVisibility component, TileExplorationStateChanged ev)
        {
            var habitants = tile.Components.Get<TileHabitants>();
            if (ev.Explored)
            {
                component.EntitiesViewing.Add(ev.Explorer);
                if (component.PlayersViewing.Add(ev.Explorer.Owner))
                {
                    ev.Explorer.Owner.OnceExplored.Add(tile);
                    ev.Explorer.Owner.VisibleTiles.Add(tile);
                    ev.Tile.Components.CallEvent(new TileVisibilityChangedEvent() { Explorer = ev.Explorer, Tile = ev.Tile, Visible = ev.Explored });
                    tile.SetFlagIncludingChildren(DeltaFlag.SELF_REVEALED);
                }
            }
            else
            {
                component.EntitiesViewing.Remove(ev.Explorer);
                if (!component.EntitiesViewing.Any(e => e.Owner == ev.Explorer.Owner))
                {
                    ev.Explorer.Owner.VisibleTiles.Remove(tile);
                    if (component.PlayersViewing.Remove(ev.Explorer.Owner))
                    {
                        ev.Tile.Components.CallEvent(new TileVisibilityChangedEvent() { Explorer = ev.Explorer, Tile = ev.Tile, Visible = ev.Explored });
                        tile.SetFlagIncludingChildren(DeltaFlag.SELF_CONCEALED);
                    }
                }
            }
        }
    }
}
