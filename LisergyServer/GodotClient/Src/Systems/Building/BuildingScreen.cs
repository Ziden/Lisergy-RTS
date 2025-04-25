using System;
using System.Threading.Tasks;
using ClientSDK;
using Game.Systems.Building;
using GameData;
using GameData.Specs;
using Godot;
using GodotClient;
using GodotClient.Services;
using LisergyGodotClient.Src.Data;
using LisergyGodotClient.Src.Systems.GameHud;
using LisergyGodotClient.Src.Systems.TechTree;
using LisergyGodotClient.Src.Systems.UiStructure;

namespace LisergyGodotClient.Src.Systems.Building;

public partial class BuildingScreen : GameUi
{
	private BuildingInfoWidget _buildingInfo;
	private Vector2 _dragStartPos;
	private ScreenHeader _header;
	private bool _isDragging;
	private ResourcesDisplayWidget _resourceDisplay;

	private IGameObject _root;
	private TechTreeItemWidget _selected;
	private GameSpec _specs;
	private TechTreeVisualLayout<BuildingSpecId> _tree;
	[Export] public NodePath Bg;
	[Export] public NodePath HeaderPath;
	[Export] public NodePath ResourcesDisplayWidget;

	[Export] public NodePath RightBar;

	public override ArtSpec GetArt()
	{
		return AssetConfigs.SCREEN_BUILDING;
	}

	public override void OnBuild()
	{
		_resourceDisplay = GetNode<ResourcesDisplayWidget>(ResourcesDisplayWidget);
		_buildingInfo = GetNode<BuildingInfoWidget>(RightBar);
		_buildingInfo.OnBuild();
		_header = GetNode<ScreenHeader>(HeaderPath);
		_header.SetData("Construction Tech Tree", () => { ClientServices.Ui.Close<BuildingScreen>(); });
		_buildingInfo.Visible = false;
		_buildingInfo.OnClickBuild = () =>
		{
			if (_selected != null)
			{
				var spec = (BuildingSpecId) _selected.Item;
				ClientServices.State.PlacingBuilding.Value = spec;
				ClientServices.Ui.Close<BuildingScreen>();
				_selected = null;
			}
		};

		_buildingInfo.OnClickClose += () =>
		{
			_buildingInfo.Visible = false;
			if (_selected != null)
			{
				_selected.SetBorder(HtmlColors.White);
				_selected = null;
			}
		};
		_root = new GodotGameObject(GetNode<TextureRect>(Bg));
		_specs = ClientServices.GameSpecs;
		_resourceDisplay.OnBuild();
	}

	public override void OnClose()
	{
		_tree.ScrollContainer.QueueFree();
		_tree = null;
		_selected = null;
		_dragStartPos = Vector2.Zero;
		_isDragging = false;
	}

	public override void OnOpen()
	{
		_buildingInfo.Visible = false;
		_tree = new TechTreeVisualLayout<BuildingSpecId>();
		_tree.CreateWidget = CreateWidget;
		_ = _tree.Draw(_root, _specs.ConstructionTechTree.Root);
	}

	private async Task<TechTreeItemWidget> CreateWidget(NodeTree<BuildingSpecId> node)
	{
		var buildingSpec = _specs.Buildings[node.Data];
		var constructionSpec = _specs.BuildingConstructions[node.Data];
		var container = await ClientServices.Ui.CreateWidget<TechTreeItemWidget>();
		container.SetData(node.Data, constructionSpec.Icon, buildingSpec.Name);
		var tech = ClientServices.LocalPlayer.EntityLogic.CheckTechTree(node.Data);
		container.SetActive(tech.Status == BuildingTechStatus.Available);
		if (tech.Status == BuildingTechStatus.NotResearched)
			container.ItemWidget.SetColor(HtmlColors.Red);
		else
			container.ItemWidget.SetColor(HtmlColors.LightGreen);
		container.OnClick = OnSelect;
		return container;
	}

	private void OnSelect(TechTreeItemWidget widget)
	{
		if (_selected != null) _selected.SetBorder(HtmlColors.White);
		widget.SetBorder(HtmlColors.LightGreen);
		_selected = widget;

		_buildingInfo.Visible = true;
		_buildingInfo.SetData((BuildingSpecId) widget.Item);
	}

	public override void _Input(InputEvent @event)
	{
		if (_tree == null) return;
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				if (mouseEvent.Pressed)
				{
					_isDragging = true;
					_dragStartPos = mouseEvent.Position;
				}
				else
				{
					_isDragging = false;
				}
			}
		}
		else if (@event is InputEventMouseMotion motionEvent && _isDragging)
		{
			var delta = _dragStartPos - motionEvent.Position;
			_tree.ScrollContainer.ScrollHorizontal += (int) Math.Round(delta.X);
			_tree.ScrollContainer.ScrollVertical += (int) Math.Round(delta.Y);
			_dragStartPos = motionEvent.Position;
		}
	}
}
