using ClientSDK.Data;
using Cysharp.Threading.Tasks;
using GameAssets;
using GameData.Specs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Assets.Code.Assets.Code.Assets
{
    public interface IAssetService : IGameService
    {
        UniTaskVoid GetAudio(SoundFX effect, Action<AudioClip> onComplete);
        UniTask<GameObject> CreateVfx(VfxPrefab t, Vector3 pos, Quaternion rot);
        UniTask<GameObject> CreateMapObject(MapObjectPrefab t, Vector3 pos, Quaternion rot);
        UniTaskVoid CreateTile(TilePrefab tile, Vector3 pos, Quaternion rot, Action<GameObject> onComplete);
        UniTask<GameObject> CreateBuilding(BuildingPrefab b, Vector3 pos, Quaternion rot);
        UniTask<GameObject> CreatePrefab(ArtSpec spec, Vector3 pos = default, Quaternion rot = default);
        UniTask PreloadAsset(ArtSpec spec);
        UniTask PreloadAsset<K>(K k) where K : IComparable, IFormattable, IConvertible;
        UniTask<Sprite> GetSprite(ArtSpec spec);
        UniTask<Texture2D> GetTexture(ArtSpec spec);
        UniTask<Sprite> GetSprite(SpritePrefab e);
        UniTask<Texture2D> GetPrefabIcon(ArtSpec spec);
        UniTask<VisualTreeAsset> GetScreen(UIScreen screen);
        UniTask<PanelSettings> GetUISetting(UISetting setting);
        UniTask<SceneInstance> LoadScene(SceneAsset scene);
        UniTask UnloadScene(SceneAsset scene);
    }

    public class AssetService : IAssetService
    {
        public static event Action<SceneAsset> OnSceneLoad;
        
        public static readonly bool SIMPLE_ASSETS = true;

        private Dictionary<SceneAsset, SceneInstance> _scenes = new();
        private AssetContainer<UISetting, PanelSettings> _uiSettings = new();
        private AssetContainer<UIScreen, VisualTreeAsset> _ui = new();
        private AssetContainer<SpritePrefab, Sprite[]> _spriteSheets = new();
        private AssetContainer<SpritePrefab, Sprite> _sprites = new();
        private AssetContainer<SpritePrefab, Texture2D> _textures = new();
        private AssetContainer<SoundFX, AudioClip> _audios = new();
        private PrefabContainer _prefabs = new();

        public async UniTaskVoid GetAudio(SoundFX fx, Action<AudioClip> onComplete)
        {
            await _audios.LoadAsync(fx, onComplete);
        }

        public async UniTask<GameObject> CreateVfx(VfxPrefab t, Vector3 pos, Quaternion rot)
        {
            return await _prefabs.InstantiateAsync(t, pos, rot, null);
        }

        public async UniTask<GameObject> CreateMapObject(MapObjectPrefab t, Vector3 pos, Quaternion rot)
        {
            return await _prefabs.InstantiateAsync(t, pos, rot, null);
        }

        public async UniTaskVoid CreateTile(TilePrefab t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
        {
            await _prefabs.InstantiateAsync(t, pos, rot, onComplete);
        }

        public UniTask<GameObject> CreateBuilding(BuildingPrefab t, Vector3 pos, Quaternion rot)
        {
            return _prefabs.InstantiateAsync(t, pos, rot, null);
        }

        public UniTask<GameObject> CreatePrefab(ArtSpec spec, Vector3 pos = default, Quaternion rot = default)
        {
            if (SIMPLE_ASSETS)
            {
                return _prefabs.InstantiateAsync(spec.Address.Split("\\").Last(), pos, rot, null);
            }

            return _prefabs.InstantiateAsync(spec.Address, pos, rot, null);
        }

        public UniTask<VisualTreeAsset> GetScreen(UIScreen screen)
        {
            return _ui.LoadAsync(screen, null);
        }

        public UniTask<Sprite> GetSprite(ArtSpec spec)
        {
            return _sprites.LoadAsync(spec.Address, null);
        }

        public UniTask<Texture2D> GetTexture(ArtSpec spec)
        {
            return _textures.LoadAsync(spec.Address, null);
        }

        public UniTask<Sprite> GetSprite(SpritePrefab fab)
        {
            return _sprites.LoadAsync(fab, null);
        }

        public UniTask<PanelSettings> GetUISetting(UISetting setting)
        {
            return _uiSettings.LoadAsync(setting, null);
        }

        public async UniTask<SceneInstance> LoadScene(SceneAsset scene)
        {
            Debug.Log("Loading scene "+scene);
            _scenes[scene] = await Addressables.LoadSceneAsync(scene.GetAddress());
            OnSceneLoad?.Invoke(scene);
            return _scenes[scene];
        }

        public async UniTask UnloadScene(SceneAsset scene)
        {
            Debug.Log("Unloading scene "+scene);
            var s = _scenes[scene];
            await Addressables.UnloadSceneAsync(s);
            _scenes.Remove(scene);
        }

        public void OnSceneLoaded()
        {
        }

        public async UniTask PreloadAsset(ArtSpec spec)
        {
            await _prefabs.LoadAsync(spec.Address);
        }

        public async UniTask PreloadAsset<K>(K k) where K : IComparable, IFormattable, IConvertible
        {
            await _prefabs.LoadAsync(k);
        }

        public UniTask<Texture2D> GetPrefabIcon(ArtSpec spec)
        {
            return UniTask.FromResult(null as Texture2D);
            //var prefab = await _prefabs.LoadAsync(spec.Address);
            //return AssetPreview.GetAssetPreview(prefab);
        }
    }
    
    public static class Extensions
    {
        public static string GetAddress<K>(this K sprite) where K : IComparable, IFormattable, IConvertible
            => AddressIdMap.IdMap[Convert.ToInt32(sprite)];
    }
}
