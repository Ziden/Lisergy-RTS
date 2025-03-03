using Game.Engine.ECLS;

namespace Game.Systems.Tile
{
    /// <summary>
    /// System that keeps track of references of which entities and which buildings are on which tiles
    /// </summary>
    [SyncedSystem]
    public class TileSystem : LogicSystem<TileDataComponent, TileLogic>
    {
        public TileSystem(LisergyGame game) : base(game) { }
        public override void RegisterListeners()
        {

        }
    }
}
