using ClientSDK;
using Game.Engine.Events.Bus;
using Game.Systems.FogOfWar;
using Game.Tile;
using Godot;
using GodotClient.Services;
using LisergyGodotClient.Data;
using LisergyGodotClient.Src.Services;
using LisergyGodotClient.Src.Systems.GameHud;

namespace LisergyGodotClient.Src.Systems.Tiles;

public class TileListener : IAutoRegisterListener
{
	private TileFog _fog;
	private IClientSdk _sdk;
	private IClientStateService _state;
	private IUiService _ui;
	
	public void OnRegister()
	{
		_fog = new TileFog(100, 100, ClientServices.Get<IGameObject>().GetNode<Node>());
		_ui = ClientServices.Get<IUiService>();
		_state = ClientServices.Get<IClientStateService>();
		_sdk = ClientServices.Get<IClientSdk>();
		_state.SelectedTile.OnChanged += State_OnTileSelected;
		_state.CameraPosition.OnChanged += State_OnCameraMoved;
		_sdk.ClientEvents.On<ClientPartyActionEvent>(this, OnPartyAction);
		_sdk.Game.Events.On<TileVisibilityChangedEvent>(this, OnTileVisibility);
	}
	private void OnTileVisibility(TileVisibilityChangedEvent e)
	{
		_fog.SetVisible(e.Tile.Entity.GetNode(), e.Visible);
		var building = e.Tile.Logic.Tile.GetBuildingOnTile();
		var entities = e.Tile.Logic.Tile.GetEntitiesOnTile();
		if (building != null) _fog.SetVisible(building.GetNode(), e.Visible);
		foreach (var entity in entities)
			if (!entity.IsMine())
				_fog.SetVisible(entity.GetNode(), e.Visible);
	}

	private void OnPartyAction(ClientPartyActionEvent e)
	{
		if (e.Action == EntityAction.CHECK && e.TargetTile != null)
			_ui.Open<TileDetailsScreen>().Then(screen => { screen.SetData(e.TargetTile); });
	}

	private void State_OnCameraMoved(Vector3 vector)
	{
		_ui.Close<TileDetailsScreen>();
	}

	private void State_OnTileSelected(TileModel model)
	{
	}
}
