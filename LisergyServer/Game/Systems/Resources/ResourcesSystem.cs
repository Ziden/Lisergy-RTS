using Game.Engine.ECLS;
using Game.Systems.Tile;

namespace Game.Systems.Resources
{
    public unsafe class ResourceSystem : LogicSystem<TileDataComponent, ResourcesLogic>
    {
        public ResourceSystem(LisergyGame game) : base(game)
        {
            EntityEvents.On<TileUpdatedEvent>(OnTileUpdated);
        }

        private void OnTileUpdated(IEntity tileEntity, TileUpdatedEvent ev)
        {
            GetLogic(tileEntity).SetTileResourcesFromHarvestSpec();
        }
    }
}
