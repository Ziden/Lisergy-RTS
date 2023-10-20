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
        UniTaskVoid GetAudio(SoundFX effect, Action<AudioClip> onComplete);
        UniTaskVoid CreateMapFX(MapFX t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete);
        UniTaskVoid CreateMapObject(MapObjectPrefab t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete);
        UniTaskVoid CreateTile(TilePrefab tile, Vector3 pos, Quaternion rot, Action<GameObject> onComplete);
        UniTaskVoid CreateBuilding(BuildingPrefab b, Vector3 pos, Quaternion rot, Action<GameObject> onComplete);
        UniTaskVoid CreatePrefab(ArtSpec spec, Vector3 pos, Quaternion rot, Action<GameObject> onComplete);
        UniTaskVoid GetSprite(ArtSpec spec, Action<Sprite> onComplete);
        UniTask<VisualTreeAsset> GetScreen(UIScreen screen);
        UniTask<PanelSettings> GetUISetting(UISetting setting);
    }
    
    public class AssetService : IAssetService
    {
        private AssetContainer<UISetting, PanelSettings> _uiSettings = new();
        private AssetContainer<UIScreen, VisualTreeAsset> _ui = new();
        private AssetContainer<SpritePrefab, Sprite[]> _spriteSheets = new();
        private AssetContainer<SpritePrefab, Sprite> _sprites = new();
        private AssetContainer<SoundFX, AudioClip> _audios = new ();
        private PrefabContainer _prefabs = new();

        public async UniTaskVoid GetAudio(SoundFX fx, Action<AudioClip> onComplete)
        {
            await _audios.LoadAsync(fx, onComplete);
        }

        public async UniTaskVoid CreateMapFX(MapFX t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
        {
            await _prefabs.InstantiateAsync(t, pos, rot, onComplete);
        }

        public async UniTaskVoid CreateMapObject(MapObjectPrefab t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
        {
            await _prefabs.InstantiateAsync(t, pos, rot, onComplete);
        }

        public async UniTaskVoid CreateTile(TilePrefab t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
        {

            await _prefabs.InstantiateAsync(t, pos, rot, onComplete);
        }

        public async UniTaskVoid CreateBuilding(BuildingPrefab t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
        {

            await _prefabs.InstantiateAsync(t, pos, rot, onComplete);
        }

        public async UniTaskVoid CreatePrefab(ArtSpec spec, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
        {
            await _prefabs.InstantiateAsync(spec.Address, pos, rot, onComplete);
        }

        public UniTask<VisualTreeAsset> GetScreen(UIScreen screen)
        {
            return _ui.LoadAsync(screen, null);      
        }

        public async UniTaskVoid GetSprite(ArtSpec spec, Action<Sprite> onComplete)
        {
            if(spec.Type == ArtType.SPRITE_SHEET)
            {
               // await _spriteSheets.LoadAsync(spec.Address, sprites => onComplete(sprites[spec.Index]));
            } else
            {
                await _sprites.LoadAsync(spec.Address, sprite => onComplete(sprite));
            }
        }

        public async UniTask<PanelSettings> GetUISetting(UISetting setting)
        {
            return await _uiSettings.LoadAsync(setting, null);
        }

        public void OnSceneLoaded() { }
    }
}
