using ClientSDK.SDKEvents;
using Game.Engine.Events.Bus;
using Game.Tile;
using GameData;
using GameData.Specs;
using Godot;
using GodotClient.Services;
using LisergyGodotClient.Src.Systems.Building;
using LisergyGodotClient.Src.Systems.Party;
using System;


namespace LisergyGodotClient.Src.Systems.GameHud
{
	public partial class GameHudScreen : GameUi, IEventListener
	{
		[Export] public NodePath _partyButton1;
		[Export] public NodePath _partyButton2;
		[Export] public NodePath _partyButton3;
		[Export] public NodePath _partyButton4;
		[Export] public NodePath _buildingButton;
		[Export] public NodePath _resources;
		
		private PartyActionBarWidget _actionBar;
		private PartySelectBarWidget _unitSelection;
		private ResourcesDisplayWidget _resourcesDisplay;
		private Button _buildButton;

		public override ArtSpec GetArt() => "res://Content/UI/Screens/GameHud.tscn";

		public override void OnBuild()
		{
			ClientServices.State.PlacingBuilding.OnChanged += State_OnPlacingBuilding;
			ClientServices.State.SelectedTile.OnChanged += State_OnTileSelected;
			ClientServices.ServerSdk.ClientEvents.On<ClientPartyActionEvent>(this, OnPartyActions);
			ClientServices.ServerSdk.ClientEvents.On<GameStartedEvent>(this, OnGameStarted);
			_unitSelection = new PartySelectBarWidget(
				GetNode<Button>(_partyButton1),GetNode<Button>(_partyButton2),
				GetNode<Button>(_partyButton3),GetNode<Button>(_partyButton4)
			);
			_buildButton = GetNode<Button>(_buildingButton);
			_buildButton.ButtonUp += OnClickBuild;
			_unitSelection.UpdateData();
			_resourcesDisplay = GetNode<ResourcesDisplayWidget>(_resources);
			_resourcesDisplay.OnBuild();
		}

		private void State_OnPlacingBuilding(BuildingSpecId id)
		{
			if(id == default)
			{
				ClientServices.Ui.Close<ConfirmBuildingDialog>();
			} else
			{
				ClientServices.Ui.Open<ConfirmBuildingDialog>().Then(ui =>
				{
					var selectedTile = ClientServices.State.SelectedTile.Value.SpecId;
					ui.SetData(id, selectedTile);
				});
			}
		}

		private void OnClickBuild()
		{
			_ = ClientServices.Ui.Open<BuildingScreen>();
		}

		private void State_OnTileSelected(TileModel obj)
		{
			if (obj == null || ClientServices.State.SelectedParty.Value == null) return;

			if (ClientServices.State.PlacingBuilding.Value != default) return;

			ClientServices.Ui.Open<PartyActionBarWidget>().Then(ui =>
			{
				ui.SetData(ClientServices.State.SelectedParty.Value, obj);
			});
		}

		public override void OnClose()
		{
			ClientServices.ServerSdk.ClientEvents.RemoveListener(this);
		}

		private void OnGameStarted(GameStartedEvent @event)
		{
			//_unitSelection.OnGameStarted(@event);
		}

		private void OnPartyActions(ClientPartyActionEvent e)
		{
			if (e.Action != EntityAction.BUILD) return;
			_ = ClientServices.Ui.Open<BuildingScreen>();
		}
	}
}
