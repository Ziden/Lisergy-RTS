using ClientSDK;
using Game.Engine.Events;
using Game.Tile;
using GameData;
using GameData.Specs;
using Godot;
using GodotClient.Services;
using LisergyGodotClient.Src.Services;
using LisergyGodotClient.Src.Systems.GameHud;
using LisergyGodotClient.Src.Systems.Tiles.UI;

namespace LisergyGodotClient.Src.Systems.Building;

public partial class ConfirmBuildingDialog : GameUi
{
	private ItemStackWidget _building;
	public Label _buildingName;
	private IGameObject _buildingSelector;
	private Button _cancelButton;
	private BuildingSpecId _specId;
	private Button _confirmButton;
	private ItemStackWidget _tile;
	public Label _tileName;
	public NodePath BuildingItem;
	public NodePath BuildingName;
	public NodePath CancelButton;
	public NodePath ConfirmButton;
	public NodePath TileItem;
	public NodePath TileName;

	public override ArtSpec GetArt()
	{
		return AssetConfigs.DIALOG_CONFIRM_BUILDING;
	}

	public override void OnBuild()
	{
		_tileName = GetNode<Label>(TileName);
		_buildingName = GetNode<Label>(BuildingName);
		_tile = GetNode<ItemStackWidget>(TileItem);
		_building = GetNode<ItemStackWidget>(BuildingItem);
		_confirmButton = GetNode<Button>(ConfirmButton);
		_cancelButton = GetNode<Button>(CancelButton);
		_confirmButton.ButtonDown += () =>
		{
			ClientServices.ServerSdk.Server.Actions.Build(
				ClientServices.State.SelectedParty.Value,
				ClientServices.State.PlacingBuilding.Value,
				ClientServices.State.SelectedTile.Value.Position);
			ClientServices.State.PlacingBuilding.Value = default;
			ClientServices.Ui.Close<ConfirmBuildingDialog>();
		};
		_cancelButton.ButtonDown += () =>
		{
			ClientServices.State.PlacingBuilding.Value = default;
			ClientServices.Ui.Close<ConfirmBuildingDialog>();
		};
	}

	
	public override void OnOpen()
	{
		ClientServices.State.SelectedTile.OnChanged += OnSelectTile;
	}

	public override void OnClose()
	{
		ClientServices.State.SelectedTile.OnChanged += OnSelectTile;
		if (_buildingSelector != null)
		{
			_buildingSelector.Destroy();
			_buildingSelector = null;
		}
	}

	private void OnSelectTile(TileModel obj)
	{
		SetData(_specId, obj.SpecId);
	}

	public void SetData(BuildingSpecId id, TileSpecId tile)
	{
		_specId = id;
		var buildingSpec = ClientServices.GameSpecs.Buildings[id];
		var constructionSpec = ClientServices.GameSpecs.BuildingConstructions[id];
		var buildStatus = ClientServices.LocalPlayer.EntityLogic.CheckTechTree(id);
		var tileSpec = ClientServices.GameSpecs.Tiles[tile];
		_building.SetData(constructionSpec.Icon, buildingSpec.Name, -1);
		_tile.SetData(tileSpec.Icon, tileSpec.Name, -1);
		_buildingName.Text = buildingSpec.Name;
		_tileName.Text = tileSpec.Name;
		_buildingName.Text = buildingSpec.Name;
	}
}
