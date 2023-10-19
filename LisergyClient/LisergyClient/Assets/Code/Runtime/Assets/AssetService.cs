using System;
using System.Threading.Tasks;
using ClientSDK.Data;
using Cysharp.Threading.Tasks;
using GameAssets;
using GameData.Specs;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code.Assets.Code.Assets
{ 
    public interface IAssetService : IGameService
    {
        UniTask GetAudio(SoundFX effect, Action<AudioClip> onComplete);
        UniTask CreateMapFX(MapFX t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete);
        UniTask CreateMapObject(MapObjectPrefab t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete);
        UniTask CreateTile(TilePrefab tile, Vector3 pos, Quaternion rot, Action<GameObject> onComplete);
        UniTask CreateBuilding(BuildingPrefab b, Vector3 pos, Quaternion rot, Action<GameObject> onComplete);
        UniTask CreatePrefab(ArtSpec spec, Vector3 pos, Quaternion rot, Action<GameObject> onComplete);
        UniTask GetSprite(ArtSpec spec, Action<Sprite> onComplete);
        UniTask GetScreen(UIScreen screen, Action<VisualTreeAsset> onComplete);
        UniTask GetUISetting(UISetting setting, Action<PanelSettings> onComplete);
    }
    
    public class AssetService : IAssetService
    {
        private AssetContainer<UISetting, PanelSettings> _uiSettings = new();
        private AssetContainer<UIScreen, VisualTreeAsset> _ui = new();
        private AssetContainer<SpritePrefab, Sprite[]> _spriteSheets = new();
        private AssetContainer<SpritePrefab, Sprite> _sprites = new();
        private AssetContainer<SoundFX, AudioClip> _audios = new ();
        private PrefabContainer _prefabs = new();

        public async UniTask GetAudio(SoundFX fx, Action<AudioClip> onComplete)
        {
            await _audios.LoadAsync(fx, onComplete);
        }

        public async UniTask CreateMapFX(MapFX t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
        {
            await _prefabs.InstantiateAsync(t, pos, rot, onComplete);
        }

        public async UniTask CreateMapObject(MapObjectPrefab t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
        {
            await _prefabs.InstantiateAsync(t, pos, rot, onComplete);
        }

        public async UniTask CreateTile(TilePrefab t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
        {

            await _prefabs.InstantiateAsync(t, pos, rot, onComplete);
        }

        public async UniTask CreateBuilding(BuildingPrefab t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
        {

            await _prefabs.InstantiateAsync(t, pos, rot, onComplete);
        }

        public async UniTask CreatePrefab(ArtSpec spec, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
        {
            await _prefabs.InstantiateAsync(spec.Address, pos, rot, onComplete);
        }

        public async UniTask GetScreen(UIScreen screen, Action<VisualTreeAsset> onComplete)
        {
            await _ui.LoadAsync(screen, onComplete);
        }

        public async UniTask GetSprite(ArtSpec spec, Action<Sprite> onComplete)
        {
            if(spec.Type == ArtType.SPRITE_SHEET)
            {
               // await _spriteSheets.LoadAsync(spec.Address, sprites => onComplete(sprites[spec.Index]));
            } else
            {
                await _sprites.LoadAsync(spec.Address, sprite => onComplete(sprite));
            }
            
        }

        public async UniTask GetUISetting(UISetting setting, Action<PanelSettings> onComplete)
        {
            await _uiSettings.LoadAsync(setting, onComplete);
        }

        public void OnSceneLoaded() { }
    }
}
