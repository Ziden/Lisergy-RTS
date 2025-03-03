using Game.Engine.ECLS;
using Game.Systems.FogOfWar;

namespace Game.Systems.Player
{
    [SyncedSystem]
    public class PlayerSystem : LogicSystem<PlayerDataComponent, PlayerLogic>
    {
        public PlayerSystem(LisergyGame game) : base(game)
        {
        }

        public override void RegisterListeners()
        {
            OnEntityEvent<TileVisibilityChangedEvent>(OnTileVisibilityChanged);
        }

        private void OnTileVisibilityChanged(IEntity e, TileVisibilityChangedEvent ev)
        {

            var tileObj = ev.Tile;
            var owner = Players.GetPlayer(ev.Explorer.OwnerID);
            if (ev.Visible)
            {
                owner.Components.Get<PlayerVisibilityComponent>().VisibleTiles.Add(tileObj.Position);
            }
            else
            {
                owner.Components.Get<PlayerVisibilityComponent>().VisibleTiles.Remove(tileObj.Position);
            }
        }
    }
}
