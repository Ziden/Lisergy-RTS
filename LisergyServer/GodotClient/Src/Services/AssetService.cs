using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientSDK;
using Game.Engine;
using Game.World;
using GameData.Specs;
using Godot;
using GodotClient;

namespace LisergyGodotClient.Src.Services;

public interface IAssetService
{
	Task<T> LoadResource<T>(ArtSpec art) where T : Resource;
	Task<CompressedTexture2D> LoadGetTexture(ArtSpec art);
	CompressedTexture2D GetTexture(ArtSpec art);
	Task<IGameObject> LoadGetArt(ArtSpec art);
	Task LoadArt(ArtSpec art);
	IGameObject GetArt(ArtSpec art);
	void AddToScene(IGameObject gameObject, Location loc = default);
	void RemoveFromScene(IGameObject o);
}

public class GodotAssetService : IAssetService
{
	private readonly Dictionary<string, PackedScene> _loadedScenes = new();
	private readonly Dictionary<string, CompressedTexture2D> _loadedTextures = new();
	private readonly IGameLog _log;
	private readonly IGameObject _root;

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
			throw new Exception("Art not loaded " + art.Address);
		var instance = _loadedScenes[art.Address].Instantiate();
		var gameObject = new GodotGameObject(instance);
		return gameObject;
	}

	public CompressedTexture2D GetTexture(ArtSpec art)
	{
		if (!_loadedTextures.TryGetValue(art.Address, out var scene))
			throw new Exception("Art not loaded " + art.Address);
		return _loadedTextures[art.Address];
	}

	public async Task LoadArt(ArtSpec art)
	{
		try
		{
			if (!_loadedScenes.TryGetValue(art.Address, out var scene))
			{
				scene = (PackedScene) await LoadGodotResource(art);
				_loadedScenes[art.Address] = scene;
				_log.Info("Loaded " + art.Address);
			}
		}
		catch (Exception e)
		{
			ClientServices.Analytics.TrackError(e);
		}
	}

	public async Task<IGameObject> LoadGetArt(ArtSpec art)
	{
		await LoadArt(art);
		return GetArt(art);
	}

	public async Task<CompressedTexture2D> LoadGetTexture(ArtSpec art)
	{
		try
		{
			if (!_loadedTextures.TryGetValue(art.Address, out var scene))
			{
				scene = (CompressedTexture2D) await LoadGodotResource(art);
				_loadedTextures[art.Address] = scene;
				_log.Info("Loaded " + art.Address);
			}

			return GetTexture(art);
		}
		catch (Exception e)
		{
			ClientServices.Analytics.TrackError(e);
			return null;
		}
	}

	public async Task<T> LoadResource<T>(ArtSpec art) where T : Resource
	{
		return (T) await LoadGodotResource(art);
	}

	public async Task<Resource> LoadGodotResource(ArtSpec art)
	{
		try
		{
			var loadResult = ResourceLoader.LoadThreadedRequest(art.Address);
			var status = ResourceLoader.LoadThreadedGetStatus(art.Address);
			while (status != ResourceLoader.ThreadLoadStatus.Loaded)
			{
				status = ResourceLoader.LoadThreadedGetStatus(art.Address);
				if (status == ResourceLoader.ThreadLoadStatus.Failed ||
				    status == ResourceLoader.ThreadLoadStatus.InvalidResource)
					throw new Exception("Invalid resource loading " + art.Address);
				await Task.Yield();
			}

			return ResourceLoader.LoadThreadedGet(art.Address);
		}
		catch (Exception e)
		{
			ClientServices.Analytics.TrackError(e);
			return null;
		}
	}
}