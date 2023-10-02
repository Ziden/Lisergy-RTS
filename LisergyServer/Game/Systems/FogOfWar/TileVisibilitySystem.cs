using Game.ECS;
using Game.Events.GameEvents;
using Game.Network;
using Game.Systems.Tile;
using Game.Tile;
using System.Linq;

namespace Game.Systems.FogOfWar
{
    public class TileVisibilitySystem : GameSystem<TileVisibility>
    {
        public override void OnEnabled()
        {
            SystemEvents.On<TileExplorationStateChanged>(OnTileExplorationChanged);
        }

        private static void OnTileExplorationChanged(IEntity tile, TileVisibility component, TileExplorationStateChanged ev)
        {
            var tileObj = (TileEntity)tile;
            var habitants = tile.Components.Get<TileHabitants>();
            if (ev.Explored)
            {
                component.EntitiesViewing.Add(ev.Explorer);
                if (component.PlayersViewing.Add(ev.Explorer.Owner))
                {
                    ev.Explorer.Owner.OnceExplored.Add(tileObj);
                    ev.Explorer.Owner.VisibleTiles.Add(tileObj);
                    ev.Tile.Components.CallEvent(new TileVisibilityChangedEvent() { Explorer = ev.Explorer, Tile = ev.Tile, Visible = ev.Explored });
                    tileObj.SetFlagIncludingChildren(DeltaFlag.SELF_REVEALED);
                }
            }
            else
            {
                component.EntitiesViewing.Remove(ev.Explorer);
                if (!component.EntitiesViewing.Any(e => e.Owner == ev.Explorer.Owner))
                {
                    ev.Explorer.Owner.VisibleTiles.Remove(tileObj);
                    if (component.PlayersViewing.Remove(ev.Explorer.Owner))
                    {
                        ev.Tile.Components.CallEvent(new TileVisibilityChangedEvent() { Explorer = ev.Explorer, Tile = ev.Tile, Visible = ev.Explored });
                        tileObj.SetFlagIncludingChildren(DeltaFlag.SELF_CONCEALED);
                    }
                }
            }
        }
    }
}
