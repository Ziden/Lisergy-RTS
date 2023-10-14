using Assets.Code.Assets.Code.Assets;
using ClientSDK.Data;
using Game.Systems.Building;

namespace Assets.Code.World
{
    public class PlayerBuildingView : UnityEntityView<PlayerBuildingEntity>
    {
        protected void InstantiationImplementation()
        {
            /*
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
            */
        }
    }
}
