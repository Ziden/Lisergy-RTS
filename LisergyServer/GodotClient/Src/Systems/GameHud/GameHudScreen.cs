using ClientSDK.SDKEvents;
using Game.Engine.Events.Bus;
using Game.Tile;
using GameData.Specs;
using Godot;
using GodotClient.Services;
using LisergyGodotClient.Src.Systems.Building;
using LisergyGodotClient.Src.Systems.Party;


namespace LisergyGodotClient.Src.Systems.GameHud
{
	public partial class GameHudScreen : GameUi, IEventListener
	{
		[Export] public NodePath _partyButton1;
		[Export] public NodePath _partyButton2;
		[Export] public NodePath _partyButton3;
		[Export] public NodePath _partyButton4;
		[Export] public NodePath _buildingButton;

		private PartyActionBarWidget _actionBar;
		private PartySelectBarWidget _unitSelection;
		private Button _buildButton;

		public override ArtSpec GetArt() => "res://Content/UI/Screens/GameHud.tscn";

		public override void OnBuild()
		{
			ClientServices.State.OnTileSelected += State_OnTileSelected;
			ClientServices.ServerSdk.ClientEvents.On<GameStartedEvent>(this, OnGameStarted);
			_unitSelection = new PartySelectBarWidget(
				GetNode<Button>(_partyButton1),GetNode<Button>(_partyButton2),
				GetNode<Button>(_partyButton3),GetNode<Button>(_partyButton4)
			);
			_buildButton = GetNode<Button>(_buildingButton);
			_buildButton.ButtonUp += OnClickBuild;
			_unitSelection.UpdateData();
		}
		
		private void OnClickBuild()
		{
			_ = ClientServices.Ui.Open<BuildingScreen>();
		}

		private void State_OnTileSelected(TileModel obj)
		{
			if (obj == null || ClientServices.State.SelectedParty == null) return;
			ClientServices.Ui.Open<PartyActionBarWidget>().Then(ui =>
			{
				ui.SetData(ClientServices.State.SelectedParty, obj);
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
	}
}
