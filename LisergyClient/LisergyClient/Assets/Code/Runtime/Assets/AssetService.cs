using System;
using System.Threading.Tasks;
using GameAssets;
using GameData.Specs;
using UnityEngine;

namespace Assets.Code.Assets.Code.Assets
{

    public interface IAssetService : IGameService
    {
        Task GetAudio(SoundFX effect, Action<AudioClip> onComplete);
        
        Task CreateTile(TilePrefab tile, Vector3 pos, Quaternion rot, Action<GameObject> onComplete);

        Task CreateTileBySpec(ArtSpec spec, Vector3 pos, Quaternion rot, Action<GameObject> onComplete);
    }
    
    public class AssetService : IAssetService
    {
        private AssetContainer<SoundFX, AudioClip> _audios = new ();
        private AssetContainer<TilePrefab, GameObject> _tiles = new ();

        public async Task GetAudio(SoundFX fx, Action<AudioClip> onComplete)
        {
            await _audios.LoadAsync(fx, onComplete);
        }
        
        public async Task CreateTile(TilePrefab t, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
        {
            await _tiles.InstantiateAsync(t, pos, rot, onComplete);
        }
        
        public async Task CreateTileBySpec(ArtSpec spec, Vector3 pos, Quaternion rot, Action<GameObject> onComplete)
        {
            await _tiles.InstantiateAsync(spec.Address, pos, rot, onComplete);
        }

        public void OnSceneLoaded() { }
    }
}
