
using Game.ECS;
using Game.Events.GameEvents;
using Game.World.Components;
using System.Linq;

namespace Game.World.Systems
{
    public class TileVisibilitySystem : GameSystem<TileVisibilityComponent, Tile>
    {
        public override void OnComponentAdded(Tile owner, TileVisibilityComponent component, ComponentEventBus<Tile> events)
        {

            events.RegisterComponentEvent<TileVisibilityChangedEvent, TileVisibilityComponent>(this, OnTileVisibilityUpdated);
        }

        private void OnTileVisibilityUpdated(Tile tile, TileVisibilityComponent component, TileVisibilityChangedEvent ev)
        {
            if (ev.Visible)
            {
                component.EntitiesViewing.Add(ev.Explorer);
                if (component.PlayersViewing.Add(ev.Explorer.Owner))
                {
                    ev.Explorer.Owner.OnceExplored.Add(tile);
                    ev.Explorer.Owner.VisibleTiles.Add(tile);
                    tile.DeltaFlags.SetFlag(DeltaFlag.SELF_REVEALED);
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
                        tile.DeltaFlags.SetFlag(DeltaFlag.SELF_CONCEALED);
                    }
                }
            }
        }
    }
}
