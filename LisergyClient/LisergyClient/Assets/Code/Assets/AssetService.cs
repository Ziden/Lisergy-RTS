using Assets.Code.Assets.Code.Audio;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Code.Assets.Code.Assets
{
    public interface IAssetService : IGameService
    {
        Task GetAudio(SoundFx effect, Action<AudioClip> onComplete = null);
        Task InstantiateTile(int tileId, Vector3 position, Action<GameObject> onComplete = null);
        Task InstantiateMapObject(string name, Vector3 position, Action<GameObject> onComplete = null);
    }

    public class AssetService : IAssetService
    {
        public static readonly string REFERENCES_ADDR = "Assets/Addressables/AssetReferences.asset";
        private static Task<AssetReferences> _referencesTask;

        private AssetContainer<SoundFx, AudioClip> _audios = new ();
        private AssetContainer<string, GameObject> _mapObjects = new();
        private AssetContainer<int, GameObject> _tileObjects = new();

        private async Task EnsureLoaded() => await _referencesTask;
        public void OnSceneLoaded() => LoadReferences();

        public async Task GetAudio(SoundFx fx, Action<AudioClip> onComplete)
        {
            await EnsureLoaded();
            await _audios.LoadAsync(fx, onComplete);
        }

        public async Task InstantiateTile(int tileId, Vector3 position, Action<GameObject> onComplete)
        {
            await EnsureLoaded();
            await _tileObjects.InstantiateAsync(tileId, position, Quaternion.Euler(90, 0, 0),  onComplete);
        }

        public async Task InstantiateMapObject(string name, Vector3 position, Action<GameObject> onComplete)
        {
            await EnsureLoaded();
            await _mapObjects.InstantiateAsync(name, position, Quaternion.Euler(90, 0, 0), onComplete);
        }

        private void LoadReferences()
        {
            var handle = Addressables.LoadAssetAsync<AssetReferences>(REFERENCES_ADDR);
            _referencesTask = handle.Task;
            handle.WaitForCompletion();
            var references = handle.Result;
            foreach (var audio in references.SoundEffects) _audios.RegisterReference(audio.Key, audio.Reference);
            foreach (var reference in references.MapObjects) _mapObjects.RegisterReference(reference.Key, reference.Reference);
        }
    }
}
