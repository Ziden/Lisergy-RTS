using Assets.Code.Views;
using ClientSDK;
using ClientSDK.Data;
using Game.Engine.ECLS;
using Game.Systems.Dungeon;
using UnityEngine;

namespace Assets.Code.World
{
    public class DungeonView : UnityEntityView
    {
        public DungeonView(IGameClient client, IEntity e) : base(e, client) { }

        protected override void CreateView()
        {
            var spec = Client.Game.Specs.Dungeons[Entity.Get<DungeonComponent>().SpecId];
            State = EntityViewState.RENDERING;
            Assets.CreatePrefab(spec.Art, Vector3.zero, Quaternion.identity, o =>
            {
                var tileView = (TileView)Client.Modules.Views.GetOrCreateView(Entity.GetTile().TileEntity);
                GameObject = o;
                tileView.SetChildren(GameObject);
                GameObject.transform.localPosition = Vector3.zero;
                GameObject.isStatic = true;
                State = EntityViewState.RENDERED;
            });
        }
    }
}
