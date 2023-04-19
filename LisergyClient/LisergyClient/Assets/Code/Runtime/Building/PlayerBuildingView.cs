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
        public override PlayerBuildingEntity Entity { get; }

        private IAssetService _assets;

        public PlayerBuildingView(PlayerBuildingEntity e)
        {
            Entity = e;
            _assets = ServiceContainer.Resolve<IAssetService>();
        }

        public override void OnUpdate(PlayerBuildingEntity serverEntity, List<IComponent> syncedComponents)
        {
            Entity.Tile = GameView.World.GetTile(serverEntity);
            if (NeedsInstantiate)
            {
                Instantiate();
            }   
        }

        protected override void InstantiationImplementation()
        {
            var spec = Entity.GetSpec();
            _assets.CreatePrefab(spec.Art, Vector3.zero, Quaternion.Euler(0, 0, 0), o =>
            {
                Debug.Log($"Instantiating building at {Entity.Tile}");
                var tile = GameView.World.GetTile(Entity);
                var tileView = GameView.GetOrCreateTileView(tile, true);
                tileView.OnInstantiate(tileObject =>
                {
                    o.SetActive(true);
                    foreach (var lod in o.GetComponentsInChildren<LODGroup>())
                    {
                        lod.ForceLOD(2);
                    }
                    StaticBatchingUtility.Combine(o);
                    o.transform.parent = tileView.GameObject.transform;
                    o.transform.localPosition = Vector3.zero;
                    SetGameObject(o);
                });
            });
        }
    }
}
