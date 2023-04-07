using System;
using System.Threading.Tasks;
using GameAssets;
using GameData.Specs;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code.Assets.Code.Assets
{
    public interface IAssetService : IGameService
    {
        Task GetAudio(SoundFX effect, Action<AudioClip> onComplete);
        Task CreateMapFX(MapFX t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete);
        Task CreateMapObject(MapObjectPrefab t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete);
        Task CreateTile(TilePrefab tile, Vector3 pos, Quaternion rot, Action<GameObject> onComplete);
        Task CreateBuilding(BuildingPrefab b, Vector3 pos, Quaternion rot, Action<GameObject> onComplete);
        Task CreatePrefab(ArtSpec spec, Vector3 pos, Quaternion rot, Action<GameObject> onComplete);
        Task GetSprite(ArtSpec spec, Action<Sprite> onComplete);
        Task GetScreen(UIScreen screen, Action<VisualTreeAsset> onComplete);
    }
    
    public class AssetService : IAssetService
    {
        private AssetContainer<UIScreen, VisualTreeAsset> _screens = new();
        private AssetContainer<SpritePrefab, Sprite[]> _sprites = new();
        private AssetContainer<SoundFX, AudioClip> _audios = new ();
        private PrefabContainer _prefabs = new();

        public async Task GetAudio(SoundFX fx, Action<AudioClip> onComplete)
        {
            await _audios.LoadAsync(fx, onComplete);
        }

        public async Task CreateMapFX(MapFX t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
        {
            await _prefabs.InstantiateAsync(t, pos, rot, onComplete);
        }

        public async Task CreateMapObject(MapObjectPrefab t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
        {
            await _prefabs.InstantiateAsync(t, pos, rot, onComplete);
        }

        public async Task CreateTile(TilePrefab t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
        {

            await _prefabs.InstantiateAsync(t, pos, rot, onComplete);
        }

        public async Task CreateBuilding(BuildingPrefab t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
        {

            await _prefabs.InstantiateAsync(t, pos, rot, onComplete);
        }

        public async Task CreatePrefab(ArtSpec spec, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
        {
            await _prefabs.InstantiateAsync(spec.Address, pos, rot, onComplete);
        }

        public async Task GetScreen(UIScreen screen, Action<VisualTreeAsset> onComplete)
        {
            await _screens.LoadAsync(screen, onComplete);
        }

        public async Task GetSprite(ArtSpec spec, Action<Sprite> onComplete)
        {
            await _sprites.LoadAsync(spec.Address, sprites => onComplete(sprites[spec.Index]));
        }

        public void OnSceneLoaded() { }
    }
}
