using System;
using Game.Engine.ECLS;
using Game.Systems.Battler;
using Game.Systems.Resources;
using Game.Systems.Tile;
using Godot;
using LisergyGodotClient.Src.Services;

// What type of company we wanna be ? Hug pullers ? I understand "that's reality"
namespace LisergyGodotClient.Src.Systems.Tiles.UI;

public partial class TileResourcesComponentWidget : Control, IEntityComponentTab
{
	private Button _harvestButton;

	private Label _rate;
	private ItemStackWidget _resourceWidget;
	private Label _total;
	[Export] public NodePath HarvestButton;
	[Export] public NodePath RatePath;
	[Export] public NodePath ResourcePath;
	[Export] public NodePath TotalPath;

	public Type ComponentType => typeof(TileResourceComponent);

	public Control Root => this;

	public void SetData(IEntity tile)
	{
		if (tile == null) return;
		var tileId = tile.Get<TileDataComponent>().TileId;
		var tileSpec = tile.Game.Specs.Tiles[tileId];
		var tileResources = tile.Get<TileResourceComponent>();
		_resourceWidget.SetData(tileResources.Resource);
		var resourceSpec = tile.Game.Specs.Resources[tileResources.Resource.ResourceId];
		var resourceSpotSpec = tile.Game.Specs.HarvestPoints[tileSpec.ResourceSpotSpecId.Value];
		var party = ClientServices.Get<IClientStateService>().SelectedParty.Value;
		var leader = party.Get<BattleGroupComponent>().Units[0];
		var unitSpec = tile.Game.Specs.Units[leader.SpecId];

		var cargo = party.Get<CargoComponent>();
		var resourcesAmount = tileResources.Resource.Amount;
		var unitsCanCarry = (ushort) (cargo.RemainingWeight / resourceSpec.WeightPerUnit);
		if (resourcesAmount > unitsCanCarry) resourcesAmount = unitsCanCarry;
		var timeToHarvest = resourcesAmount * resourceSpotSpec.HarvestTimePerUnit;
		_harvestButton.Disabled = resourcesAmount == 0;

		_rate.Text =
			$"Harvest for {unitSpec.Name} \n 1 {resourceSpec.Name} \n every {resourceSpotSpec.HarvestTimePerUnit.ToReadableString()}";
		_total.Text = $"Total {timeToHarvest.ToReadableString()}";
	}

	public override void _Ready()
	{
		_resourceWidget = Root.GetNode<ItemStackWidget>(ResourcePath);
		_rate = Root.GetNode<Label>(RatePath);
		_total = Root.GetNode<Label>(TotalPath);
		_harvestButton = Root.GetNode<Button>(HarvestButton);
	}
}