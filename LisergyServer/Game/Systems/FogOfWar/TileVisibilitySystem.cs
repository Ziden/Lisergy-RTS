using Game.ECS;
using Game.Events.GameEvents;
using Game.Network;
using Game.Systems.Tile;
using Game.Tile;
using System.Linq;

namespace Game.Systems.FogOfWar
{
    public class TileVisibilitySystem : GameSystem<TileVisibility, TileEntity>
    {
        public override void OnEnabled()
        {
            SystemEvents.On<TileExplorationStateChanged>(OnTileExplorationChanged);
        }

        private static void OnTileExplorationChanged(TileEntity tile, TileVisibility component, TileExplorationStateChanged ev)
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
