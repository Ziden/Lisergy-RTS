using Game.ECS;
using Game.Events.GameEvents;
using Game.Network;
using Game.Systems.Tile;
using Game.Tile;
using Game.World;
using System.Linq;

namespace Game.Systems.FogOfWar
{
    public class TileVisibilitySystem : GameSystem<TileVisibility>
    {
        public TileVisibilitySystem(LisergyGame game) : base(game) { }
        public override void OnEnabled()
        {
            EntityEvents.On<TileExplorationStateChanged>(OnTileExplorationChanged);
        }

        private void OnTileExplorationChanged(IEntity tile, ref TileVisibility component, TileExplorationStateChanged ev)
        {
            var tileObj = (TileEntity)tile;
            var habitants = tile.Components.Get<TileHabitants>();
            if (ev.Explored)
            {
                component.EntitiesViewing.Add(ev.Explorer);

                var owner = Players.GetPlayer(ev.Explorer.OwnerID);
                if (component.PlayersViewing.Add(owner))
                {
                    owner.Data.OnceExplored.Add(tileObj);
                    owner.Data.VisibleTiles.Add(tileObj);
                    ev.Tile.Components.CallEvent(new TileVisibilityChangedEvent() { Explorer = ev.Explorer, Tile = ev.Tile, Visible = ev.Explored });
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
                        ev.Tile.Components.CallEvent(new TileVisibilityChangedEvent() { Explorer = ev.Explorer, Tile = ev.Tile, Visible = ev.Explored });
                        tileObj.SetFlag(DeltaFlag.SELF_CONCEALED);
                    }
                }
            }
        }
    }
}
