using ClientSDK;
using ClientSDK.Data;
using Game.Engine.ECLS;
using Game.Systems.Tile;
using Godot;
using GodotClient;
using System.Threading.Tasks;

namespace LisergyGodotClient.Src.Systems.Tiles
{
    public class TileView : EntityView
    {

        private Node _tileNode;

        public TileView(IEntity entity, IClientSDK client) : base(entity, client)
        {
            
        }

        public void UpdateFog(bool visible)
        {
        }

        protected override async Task CreateView()
        {
            var data = Entity.Get<TileDataComponent>();
            var spec = Client.Game.Specs.Tiles[data.TileId];
            GameObject = await ClientServices.Assets.LoadGetArt(spec.Model);
            _tileNode = ((GodotGameObject)GameObject).Node;
            ClientServices.Assets.AddToScene(GameObject, data.Position); 
        }
    }
}
