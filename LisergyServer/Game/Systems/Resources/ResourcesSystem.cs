using Game.Engine.ECS;
using Game.Systems.Tile;

namespace Game.Systems.Resources
{
    public unsafe class ResourceSystem : LogicSystem<TileComponent, ResourcesLogic>
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
