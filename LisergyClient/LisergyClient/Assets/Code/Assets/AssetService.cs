using Assets.Code.Assets.Code.Audio;
using Game;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Assets.Code.Assets.Code.Assets
{

    public interface IAssetService : IGameService
    {
        Task GetAudio(SoundFx effect, Action<AudioClip> onComplete);
    }


    public class AssetService : IAssetService
    {
        public static readonly string REFERENCES_ADDR = "Assets/Addressables/AssetReferences.asset";
        private static Task<AssetReferences> _referencesTask;

        private AssetContainer<SoundFx, AudioClip> _audios = new ();

        public async Task GetAudio(SoundFx fx, Action<AudioClip> onComplete)
        {
            await EnsureLoaded();
            await _audios.LoadAsync(fx, onComplete);
        }

        private async Task EnsureLoaded() => await _referencesTask;

        public void OnSceneLoaded() => _ = LoadReferences();

        private async Task LoadReferences()
        {
            var handle = Addressables.LoadAssetAsync<AssetReferences>(REFERENCES_ADDR);
            _referencesTask = handle.Task;
            var references = await _referencesTask;
            foreach (var audio in references.SoundEffects) _audios.RegisterReference(audio.Key, audio.Reference);
        }
    }
}
