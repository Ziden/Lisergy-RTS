using Game.ECS;
using Game.Events;
using Game.Network;
using Game.Systems.Tile;
using System.Linq;

namespace Game.Systems.FogOfWar
{
    [SyncedSystem]
    public class TileVisibilitySystem : GameSystem<TileComponent>
    {
        public TileVisibilitySystem(LisergyGame game) : base(game) { }
        public override void RegisterListeners()
        {
            EntityEvents.On<EntityTileVisibilityUpdateEvent>(OnTileExplorationChanged);
        }

       
        private void OnTileExplorationChanged(IEntity tile, EntityTileVisibilityUpdateEvent ev)
        {
            var tileObj = ev.Tile;
            var component = tile.Components.GetReference<TileVisibility>();
            var habitants = tile.Components.GetReference<TileHabitantsReferenceComponent>();
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
