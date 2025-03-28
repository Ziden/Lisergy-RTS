using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using ClientSDK;
using Cysharp.Threading.Tasks;
using Game.Engine;
using Game.World;
using GameData.Specs;
using Godot;
using GodotClient;
using LisergyGodotClient.Src;
using static System.Formats.Asn1.AsnWriter;

namespace LisergyGodotClient.Src.Services
{


    public interface IAssetService
    {
        UniTask<T> LoadResource<T>(ArtSpec art) where T : Resource;
        UniTask<CompressedTexture2D> LoadGetTexture(ArtSpec art);
        CompressedTexture2D GetTexture(ArtSpec art);
        UniTask<IGameObject> LoadGetArt(ArtSpec art);
        UniTask LoadArt(ArtSpec art);
        IGameObject GetArt(ArtSpec art);
        void AddToScene(IGameObject gameObject, Location loc = default);
        void RemoveFromScene(IGameObject o);
    }

    public class GodotAssetService : IAssetService
    {
        private IGameObject _root;
        private IGameLog _log;

        private Dictionary<string, PackedScene> _loadedScenes = new Dictionary<string, PackedScene>();
        private Dictionary<string, CompressedTexture2D> _loadedTextures = new Dictionary<string, CompressedTexture2D>();

        public GodotAssetService(IGameObject root, IGameLog log)
        {
            _root = root;
            _log = log;
        }

        public void RemoveFromScene(IGameObject gameObject)
        {
            _root.DestroyChild(gameObject);
        }

        public void AddToScene(IGameObject gameObject, Location loc = default)
        {
            _root.AddChild(gameObject);
            gameObject.Location = loc;
        }

        public IGameObject GetArt(ArtSpec art)
        {
            if (!_loadedScenes.TryGetValue(art.Address, out var scene))
            {
                throw new Exception("Art not loaded " + art.Address);
            }
            var instance = _loadedScenes[art.Address].Instantiate();
            var gameObject = new GodotGameObject(instance);
            return gameObject;
        }

        public CompressedTexture2D GetTexture(ArtSpec art)
        {
            if (!_loadedTextures.TryGetValue(art.Address, out var scene))
            {
                throw new Exception("Art not loaded " + art.Address);
            }
            return _loadedTextures[art.Address];
        }

        public async UniTask LoadArt(ArtSpec art)
        {
            if (!_loadedScenes.TryGetValue(art.Address, out var scene))
            {
                scene = (PackedScene)await LoadGodotResource(art);
                _loadedScenes[art.Address] = scene;
                _log.Info("Loaded " + art.Address);
            }
        }

        public async UniTask<IGameObject> LoadGetArt(ArtSpec art)
        {
            await LoadArt(art);
            return GetArt(art);
        }

        public async UniTask<CompressedTexture2D> LoadGetTexture(ArtSpec art)
        {
            if (!_loadedTextures.TryGetValue(art.Address, out var scene))
            {
                scene = (CompressedTexture2D) await LoadGodotResource(art);
                _loadedTextures[art.Address] = scene;
                _log.Info("Loaded " + art.Address);
            }
            return GetTexture(art);
        }

        public async UniTask<Resource> LoadGodotResource(ArtSpec art)
        {
            var loadResult = ResourceLoader.LoadThreadedRequest(art.Address);
            var status = ResourceLoader.LoadThreadedGetStatus(art.Address);
            while (status != ResourceLoader.ThreadLoadStatus.Loaded)
            {
                status = ResourceLoader.LoadThreadedGetStatus(art.Address);
                if (status == ResourceLoader.ThreadLoadStatus.Failed || status == ResourceLoader.ThreadLoadStatus.InvalidResource)
                {
                    throw new Exception("Invalid resource loading " + art.Address);
                }
                await UniTask.Yield();
            }
            return ResourceLoader.LoadThreadedGet(art.Address);
        }

        public async UniTask<T> LoadResource<T>(ArtSpec art) where T : Resource
        {
            return (T) await LoadGodotResource(art);
        }
    }
}
