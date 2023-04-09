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
        Task GetUISetting(UISetting setting, Action<PanelSettings> onComplete);
    }
    
    public class AssetService : IAssetService
    {
        private AssetContainer<UISetting, PanelSettings> _uiSettings = new();
        private AssetContainer<UIScreen, VisualTreeAsset> _screens = new();
        private AssetContainer<SpritePrefab, Sprite[]> _spriteSheets = new();
        private AssetContainer<SpritePrefab, Sprite> _sprites = new();
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
            if(spec.Type == ArtType.SPRITE_SHEET)
            {
                await _spriteSheets.LoadAsync(spec.Address, sprites => onComplete(sprites[spec.Index]));
            } else
            {
                await _sprites.LoadAsync(spec.Address, sprite => onComplete(sprite));
            }
            
        }

        public async Task GetUISetting(UISetting setting, Action<PanelSettings> onComplete)
        {
            await _uiSettings.LoadAsync(setting, onComplete);
        }

        public void OnSceneLoaded() { }
    }
}
