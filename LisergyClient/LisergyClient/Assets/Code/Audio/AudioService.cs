
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

        private Dictionary<SoundFx, AudioClip> _fx = new Dictionary<SoundFx, AudioClip>();

        private void Load()
        {
            _fx[SoundFx.BUTTON_CLICK] = Resources.Load("Audio/Sfx/button_click") as AudioClip;
        }

        private AudioClip GetClip(SoundFx fx)
        {
            return _fx[fx];
        }

        public AudioService()
        {
            var audio = new GameObject("Sfx");
            _audioSource = audio.AddComponent<AudioSource>();
            Load();
        }

        public void PlaySoundEffect(SoundFx fx, float volume = 1f)
        {
            _audioSource.PlayOneShot(GetClip(fx), volume);
        }
    }
}
