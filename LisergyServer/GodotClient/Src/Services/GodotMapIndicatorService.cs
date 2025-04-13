using ClientSDK;
using ClientSDK.SDKEvents;
using Game.Engine.ECLS;
using Game.Engine.Events.Bus;
using Game.Tile;
using Game.World;
using Godot;
using System.Threading.Tasks;

namespace LisergyGodotClient.Src.Services
{
	/// <summary>
	/// This service is responsible for moving the tile and unit selectors on the map
	/// </summary>
	public interface IMapIndicatorService { }

	public class GodotMapIndicatorService : IMapIndicatorService, IEventListener
	{
		private IClientSdk _sdk;
		private IClientStateService _state;
		private IAssetService _assets;

		private IGameObject _tileSelector;
		private IGameObject _unitSelector;

		public GodotMapIndicatorService(IClientSdk sdk, IAssetService assets, IClientStateService state)
		{
			_sdk = sdk;
			_assets = assets;
			_state = state;

			state.SelectedTile.OnChanged += OnTileSelected;
			state.CameraPosition.OnChanged += OnCameraMoved;
			state.SelectedParty.OnChanged += OnPartySelected;
			_sdk.ClientEvents.On<GameStartedEvent>(this, OnGameStarted);
		}

	

		private void OnGameStarted(GameStartedEvent e)
		{
			_ = Initialize();
		}

		private async Task Initialize()
		{
			var art = await _assets.LoadGetArt(AssetConfigs.MODEL_TILE_SELECTOR);
			_assets.AddToScene(art);
			_tileSelector = art;
			if (_state.SelectedTile.Value != null)
			{
				MoveTileSelector(_state.SelectedTile.Value.Position);
			} else
			{
				MoveTileSelector(new Location(-999, -999));
			}

			var art2 = await _assets.LoadGetArt(AssetConfigs.MODEL_UNIT_SELECTOR);
			_assets.AddToScene(art2);
			_unitSelector = art2;
			if (_state.SelectedParty.Value != null)
			{
				MovePartySelector(_state.SelectedParty.Value, _state.SelectedTile.Value.Position);
			} else
			{
				MovePartySelector(null, new Location(-999, -999));
			}
		}

		private void OnPartySelected(IEntity e)
		{
			if(e == null)
			{
				MovePartySelector(null, new Location(-999, -999));
			} else
			{
				MovePartySelector(e, e.GetTile().Position);
			}
		}

		private void MoveTileSelector(Location tile)
		{
			if (_tileSelector == null) return;
			_tileSelector.Location = tile;
		}

		private void MovePartySelector(IEntity entity, Location tile)
		{
			if (_unitSelector == null) return;

			if(entity == null)
			{
				_unitSelector.Visible = false;
			} else
			{
				_unitSelector.Visible = true;
				entity.GetView().GameObject.AddChild(_unitSelector);
			}
		}

		private void OnTileSelected(TileModel tile)
		{
			MoveTileSelector(tile.Position);
		}

		private void OnCameraMoved(Vector3 pos)
		{
		}
	}
}
