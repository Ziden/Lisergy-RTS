using Game.ECS;
using Game.Events;
using Game.Events.GameEvents;
using Game.Network;
using Game.Systems.Tile;
using Game.Tile;
using System.Linq;
using System.Runtime.CompilerServices;

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

       
        private void OnTileExplorationChanged(IEntity tile, EntityTileVisibilityUpdateEvent ev)
        {
            var tileObj = ev.Tile;
            var component = tile.Components.GetReference<TileVisibility>();
            var habitants = tile.Components.GetReference<TileHabitants>();
            var owner = Players.GetPlayer(ev.Explorer.OwnerID);
            if (ev.Explored)
            {
                component.EntitiesViewing.Add(ev.Explorer);
               
                if (component.PlayersViewing.Add(owner))
                {
                    var e = EventPool<TileVisibilityChangedForPlayerEvent>.Get();
                    e.Explorer = ev.Explorer;
                    e.Tile = ev.Tile;
                    e.Visible = ev.Explored;
                    owner.Components.CallEvent(e);
                    EventPool<TileVisibilityChangedForPlayerEvent>.Return(e);
                    tileObj.SetDeltaFlag(DeltaFlag.SELF_REVEALED);
                }
            }
            else
            {
                component.EntitiesViewing.Remove(ev.Explorer);
                if (!component.EntitiesViewing.Any(e => e.OwnerID == ev.Explorer.OwnerID))
                {
                    if (component.PlayersViewing.Remove(owner))
                    {
                        var e = EventPool<TileVisibilityChangedForPlayerEvent>.Get();
                        e.Explorer = ev.Explorer;
                        e.Tile = ev.Tile;
                        e.Visible = ev.Explored;
                        owner.Components.CallEvent(e);
                        EventPool<TileVisibilityChangedForPlayerEvent>.Return(e);
                        tileObj.SetDeltaFlag(DeltaFlag.SELF_CONCEALED);
                    }
                }
            }
        }
    }
}
