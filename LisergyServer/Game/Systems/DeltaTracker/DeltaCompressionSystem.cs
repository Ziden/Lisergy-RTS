using Game.Engine.ECLS;
using Game.Systems.DeltaTracker;
using Game.Systems.FogOfWar;

namespace Game.Systems.Dungeon
{
    public class DeltaCompressionSystem : LogicSystem<DeltaFlagsComponent, DeltaCompressionLogic>
    {
        public DeltaCompressionSystem(LisergyGame game) : base(game)
        {
        }

        public override void RegisterListeners()
        {
            OnAnyEvent<TileVisibilityChangedEvent>(OnTileVisibilityChanged);
        }


        private void OnTileVisibilityChanged(TileVisibilityChangedEvent ev)
        {
            if (ev.Visible)
            {
                GetLogic(ev.Tile.TileEntity).SetTileExplorationFlag(DeltaFlag.SELF_REVEALED);
            }
            else
            {
                GetLogic(ev.Tile.TileEntity).SetTileExplorationFlag(DeltaFlag.SELF_CONCEALED);
            }
        }

    }
}
