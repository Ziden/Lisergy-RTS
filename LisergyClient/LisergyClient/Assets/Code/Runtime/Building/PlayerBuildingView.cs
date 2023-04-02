using Assets.Code.Assets.Code.Assets;
using Assets.Code.Views;
using Game;
using Game.Building;
using Game.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.World
{
    public class PlayerBuildingView : EntityView<PlayerBuildingEntity>
    {

        public override bool Instantiated => GameObject != null;
        public override PlayerBuildingEntity Entity { get; }
        public override GameObject GameObject { get; set; }

        private IAssetService _assets;

        public PlayerBuildingView(PlayerBuildingEntity e)
        {
            Entity = e;
            _assets = ServiceContainer.Resolve<IAssetService>();
        }

        public override void OnUpdate(PlayerBuildingEntity serverEntity, List<IComponent> syncedComponents)
        {
            Entity.Tile = GameView.World.GetTile(serverEntity);
            if (!Instantiated)
            {
                Instantiate();
            }   
        }

        public override void Instantiate()
        {
            var spec = Entity.GetSpec();
            _assets.CreatePrefab(spec.Art, Vector3.zero, Quaternion.Euler(0, 0, 0), o =>
            {
                var tile = GameView.World.GetTile(Entity);
                var tileView = GameView.GetOrCreateTileView(tile, true);
                GameObject = o;
                GameObject.transform.parent = tileView.GameObject.transform;
                GameObject.transform.localPosition = Vector3.zero;

                foreach (var lod in GameObject.GetComponentsInChildren<LODGroup>())
                {
                    lod.ForceLOD(2);
                }
                StaticBatchingUtility.Combine(GameObject);
                this.GameObject.SetActive(true);
            });
        }
    }
}
