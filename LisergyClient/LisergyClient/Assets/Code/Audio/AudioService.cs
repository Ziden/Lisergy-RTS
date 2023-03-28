
using Assets.Code.Assets.Code.Assets;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Assets.Code.Audio
{
    public enum SoundFx
    {
        BUTTON_CLICK
    }

    public interface IAudioService : IGameService
    {
        void PlaySoundEffect(SoundFx fx, float volume = 1f);
    }


    public class AudioService : IAudioService
    {
        private AudioSource _audioSource;

        private IAssetService _assets;

        public void OnSceneLoaded()
        {
            var audio = new GameObject("Sfx");
            _audioSource = audio.AddComponent<AudioSource>();
            _assets = ServiceContainer.Resolve<IAssetService>();
        }

        public void PlaySoundEffect(SoundFx fx, float volume = 1f)
        {
            _assets.GetAudio(fx, audio => _audioSource.PlayOneShot(audio, volume));
        }
    }
}
