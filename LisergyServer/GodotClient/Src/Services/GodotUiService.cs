using ClientSDK;
using Game.Engine;
using GameData.Specs;
using Godot;
using LisergyGodotClient.Src;
using LisergyGodotClient.Src.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GodotClient.Services
{
	public abstract partial class GameUiWidget : Control
	{
		public IGameObject GameObject { get; internal set; }
		public abstract ArtSpec GetArt();
		public virtual void OnBuild() { }
	}

	public abstract partial class GameUi : GameUiWidget
	{
		public virtual void OnBeforeOpen() { }
		public virtual void OnOpen() { }
		public virtual void OnClose() { }
	}

	public interface IUiService
	{
		Task<T> Open<T>() where T : GameUi, new();
		void Close<T>() where T : GameUi;
		void Destroy<T>() where T : GameUi;
		T Get<T>() where T : GameUi;
		bool IsOpen<T>() where T : GameUi;
		Task<T> CreateWidget<T>() where T : GameUiWidget, new();
	}

	public class GodotUiService : IUiService
	{
		private IGameObject _root;
		private IClientSdk _client;
		private IAssetService _assets;
		private IGameLog _log;

		private Dictionary<Type, GameUi> _loadedScreens = new Dictionary<Type, GameUi>();
		private Dictionary<Type, GameUi> _openScreens = new Dictionary<Type, GameUi>();

		public GodotUiService(IClientSdk client, IAssetService assets, IGameObject root, IGameLog log)
		{
			_log = log;
			_client = client;
			_assets = assets;
			_root = root;
		}

		public async Task<T> Open<T>() where T : GameUi, new()
		{
			try
			{
				var t = typeof(T);
				if (_openScreens.TryGetValue(t, out var uiScreen))
					return (T)uiScreen;

				if (!_loadedScreens.TryGetValue(t, out var loadedScreen))
				{
					_log.Info("Loading screen " + t);
					loadedScreen = await CreateWidget<T>();
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
			} catch(Exception e)
			{
				ClientServices.Analytics.TrackError(e);
				return null;
			}
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

		public void Destroy<T>() where T : GameUi
		{
			if (_loadedScreens.ContainsKey(typeof(T)))
			{
				_root.DestroyChild(_loadedScreens[typeof(T)].GameObject);
				_loadedScreens.Remove(typeof(T));
			}
			if (_openScreens.ContainsKey(typeof(T)))
			{
				_openScreens.Remove(typeof(T));
			}
		}

		// TODO: Improve
		public async Task<T> CreateWidget<T>() where T : GameUiWidget, new()
		{
			try
			{
				var gameObject = await _assets.LoadGetArt(new T().GetArt());
				var loadedScreen = gameObject.Get<GameUiWidget>();
				loadedScreen.GameObject = gameObject;
				return (T)loadedScreen;
			}
			catch (Exception e)
			{
				ClientServices.Analytics.TrackError(e);
				return null;
			}
		}
	}
}
