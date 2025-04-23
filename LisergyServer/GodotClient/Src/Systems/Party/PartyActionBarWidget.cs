using System;
using System.Collections.Generic;
using System.Linq;
using Game.Engine.ECLS;
using Game.Systems.Map;
using Game.Tile;
using GameData.Specs;
using Godot;
using GodotClient.Services;

namespace LisergyGodotClient.Src.Systems.GameHud;

public enum EntityAction
{
	NONE,
	MOVE,
	ATTACK,
	CHECK,
	HARVEST,
	BUILD
}

public partial class PartyActionBarWidget : GameUi
{
	private Button _attackButton;
	private Button _buildButton;
	private Camera3D _camera;

	private Control _centerNode;
	private Button _checkButton;
	private Button _harvestButton;
	private Button _moveButton;

	private IEntity _targetEntity;
	private Node3D _targetEntityNode;
	private TileModel _targetTile;
	[Export] public NodePath AttackButton;
	[Export] public NodePath BuildButton;
	[Export] public NodePath Center;
	[Export] public NodePath CheckButton;
	[Export] public NodePath HarvestButton;
	[Export] public NodePath MoveButton;

	public override ArtSpec GetArt()
	{
		return AssetConfigs.WIDGET_PARTY_ACTIONS;
	}

	public override void _Process(double delta)
	{
		if (_targetEntityNode != null && _camera != null)
		{
			var screenPosition = _camera.UnprojectPosition(_targetEntityNode.GlobalPosition);
			GlobalPosition = screenPosition;
		}
	}

	public override void OnBuild()
	{
		_buildButton = GetNode<Button>(BuildButton);
		_centerNode = GetNode<Control>(Center);
		_camera = ClientServices.Get<Camera3D>();
		_moveButton = GetNode<Button>(MoveButton);
		_attackButton = GetNode<Button>(AttackButton);
		_checkButton = GetNode<Button>(CheckButton);
		_harvestButton = GetNode<Button>(HarvestButton);
		ClientServices.State.CameraPosition.OnChanged += State_OnCameraMoved;

		_buildButton.ButtonDown += () => OnActionChosen(EntityAction.BUILD);
		_moveButton.ButtonDown += () => OnActionChosen(EntityAction.MOVE);
		_attackButton.ButtonDown += () => OnActionChosen(EntityAction.ATTACK);
		_checkButton.ButtonDown += () => OnActionChosen(EntityAction.CHECK);
		_harvestButton.ButtonDown += () => OnActionChosen(EntityAction.HARVEST);
	}

	private void OnActionChosen(EntityAction action)
	{
		ClientServices.ServerSdk.ClientEvents.Call(new ClientPartyActionEvent
		{
			Action = action,
			TargetEntity = _targetEntity,
			TargetTile = _targetTile
		});
		ClientServices.Ui.Close<PartyActionBarWidget>();
	}

	public override void OnClose()
	{
		ClientServices.State.CameraPosition.OnChanged -= State_OnCameraMoved;
	}

	private void State_OnCameraMoved(Vector3 vector)
	{
		_targetEntityNode = null;
		_targetEntity = null;
		ClientServices.Ui.Close<PartyActionBarWidget>();
	}

	public void SetData(IEntity party, TileModel tile)
	{
		var partyNode = tile.Entity.GetView().GameObject.GetNode<Node3D>();
		_targetEntityNode = partyNode;
		_targetTile = tile;
		_targetEntity = party;
		var actions = EvaluateActions(party, tile);
		_moveButton.Visible = actions.Contains(EntityAction.BUILD);
		_moveButton.Visible = actions.Contains(EntityAction.MOVE);
		_attackButton.Visible = actions.Contains(EntityAction.ATTACK);
		_checkButton.Visible = actions.Contains(EntityAction.CHECK);
		_harvestButton.Visible = actions.Contains(EntityAction.HARVEST);
	}

	private EntityAction[] EvaluateActions(IEntity party, TileModel targetTile)
	{
		if (!party.Components.TryGet<MapPlacementComponent>(out var placed)) return Array.Empty<EntityAction>();
		var partyTile = party.GetTile();
		var tileView = ClientServices.ServerSdk.Server.Views.GetEntityView(targetTile.Entity);
		var actions = new HashSet<EntityAction>();
		var buildingOnTile = tileView.Entity.Logic.Tile.GetBuildingOnTile();
		actions.Add(EntityAction.CHECK);
		if (buildingOnTile != null)
		{
			if (!buildingOnTile.OwnerID.IsMine())
				actions.Add(EntityAction.ATTACK);
			else if (buildingOnTile.Logic.Building.IsConstruction()) actions.Add(EntityAction.BUILD);
			return actions.ToArray();
		}

		actions.Add(EntityAction.MOVE);
		if (party.Logic.Harvesting.GetAvailableResourcesToHarvest(targetTile).Amount > 0)
			actions.Add(EntityAction.HARVEST);
		return actions.ToArray();
	}
}
