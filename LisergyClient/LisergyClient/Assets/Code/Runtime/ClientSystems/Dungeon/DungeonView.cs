using Assets.Code.Views;
using ClientSDK;
using ClientSDK.Data;
using Game.Engine.ECLS;
using Game.Systems.Dungeon;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.World
{
    public class DungeonView : UnityEntityView
    {
        public DungeonView(IGameClient client, IEntity e) : base(e, client) { }

        protected override async Task CreateView()
        {
            var spec = Client.Game.Specs.Dungeons[Entity.Get<DungeonComponent>().SpecId];
            State = EntityViewState.RENDERING;
            var o = await Assets.CreatePrefab(spec.Art);
            var tileView = (TileView)Client.Modules.Views.GetOrCreateView(Entity.GetTile().Entity);
            GameObject = o;
            tileView.SetChildren(GameObject);
            GameObject.transform.localPosition = Vector3.zero;
            State = EntityViewState.RENDERED;
        }
    }
}
