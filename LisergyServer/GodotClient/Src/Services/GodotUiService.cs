using ClientSDK;
using Cysharp.Threading.Tasks;
using Game.Engine;
using GameData.Specs;
using Godot;
using LisergyGodotClient.Src.Services;
using System;
using System.Collections.Generic;

namespace GodotClient.Services
{
	public abstract partial class GameUi : Control
	{
		public IGameObject GameObject { get; internal set; }
		public abstract ArtSpec GetArt();

		public virtual void OnBuild() { }
		public virtual void OnBeforeOpen() { }
		public virtual void OnOpen() { }
		public virtual void OnClose() { }
	}

	public interface IUiService
	{
		UniTask<T> Open<T>() where T : GameUi, new();
		void Close<T>() where T : GameUi;
		T Get<T>() where T : GameUi;
		bool IsOpen<T>() where T : GameUi;
	}

	public class GodotUiService : IUiService
	{
		private IGameObject _root;
		private IClientSDK _client;
		private IAssetService _assets;
		private IGameLog _log;

		private Dictionary<Type, GameUi> _loadedScreens = new Dictionary<Type, GameUi>();
		private Dictionary<Type, GameUi> _openScreens = new Dictionary<Type, GameUi>();

		public GodotUiService(IClientSDK client, IAssetService assets, IGameObject root, IGameLog log)
		{
			_log = log;
			_client = client;
			_assets = assets;
			_root = root;
		}

		public async UniTask<T> Open<T>() where T : GameUi, new()
		{
			var t = typeof(T);
			if (_openScreens.TryGetValue(t, out var uiScreen))
				return (T)uiScreen;

			if (!_loadedScreens.TryGetValue(t, out var loadedScreen))
			{
				_log.Info("Loading screen " + t);
				var gameObject = await _assets.LoadGetArt(new T().GetArt()); // TODO: Improve
				loadedScreen = gameObject.Get<GameUi>();
				loadedScreen.GameObject = gameObject;
				_loadedScreens[t] = loadedScreen;
				_root.AddChild(loadedScreen.GameObject);
				loadedScreen.OnBuild();
			}
			_log.Info("Opening screen " + t);
			loadedScreen.OnBeforeOpen();
			_openScreens[t] = loadedScreen;
			loadedScreen.Visible = true;
			loadedScreen.OnOpen();
			return (T)loadedScreen;
		}

		public void Close<T>() where T : GameUi
		{
			if (!_openScreens.ContainsKey(typeof(T)))
				return;

			_log.Info("Closing screen " + typeof(T));
			GameUi screen = _openScreens[typeof(T)];
			screen.OnClose();
			screen.GameObject.Visible = false;
			_openScreens.Remove(typeof(T));
		}

		public T Get<T>() where T : GameUi
		{
			if (_openScreens.ContainsKey(typeof(T)))
				return (T)_openScreens[typeof(T)];
			return null;
		}

		public bool IsOpen<T>() where T : GameUi
		{
			return _openScreens.ContainsKey(typeof(T));
		}
	}
}
