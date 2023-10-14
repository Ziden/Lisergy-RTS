
using Assets.Code.Assets.Code.Assets;
using System.Collections.Generic;
using GameAssets;
using UnityEngine;
using ClientSDK;
using ClientSDK.Data;

namespace Assets.Code.Assets.Code.Audio
{
    public interface IAudioService : IGameService
    {
        void PlaySoundEffect(SoundFX fx, float volume = 1f);
    }


    public class AudioService : IAudioService
    {
        private AudioSource _audioSource;

        private IAssetService _assets;
        private IGameClient _client;

        public AudioService(IGameClient client)
        {
            _client = client;
        }


        public void OnSceneLoaded()
        {
            var audio = new GameObject("Sfx");
            _audioSource = audio.AddComponent<AudioSource>();
            _assets = ServiceContainer.Resolve<IAssetService>();
        }

        public void PlaySoundEffect(SoundFX fx, float volume = 1f)
        {
            _assets.GetAudio(fx, audio => _audioSource.PlayOneShot(audio, volume));
        }
    }
}
