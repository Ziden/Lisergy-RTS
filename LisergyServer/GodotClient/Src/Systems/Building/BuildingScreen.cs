using Godot;
using GameData;
using GodotClient;
using System;
using LisergyGodotClient.Src.Systems.TechTree;
using GodotClient.Services;
using GameData.Specs;
using Game.Systems.Building;
using LisergyGodotClient.Src.Data;
using System.Threading.Tasks;
using ClientSDK;
using LisergyGodotClient.Src.Systems.Tiles.UI;
using LisergyGodotClient.Src.Systems.UiStructure;


namespace LisergyGodotClient.Src.Systems.Building
{
	public partial class BuildingScreen : GameUi
	{
		[Export] public NodePath HeaderPath;
		[Export] public NodePath Bg;
		[Export] public NodePath CostsGrid;
		[Export] public NodePath NameLabel;
		[Export] public NodePath DescLabel;
		[Export] public NodePath BuildTimeLabel;
		[Export] public NodePath BuildButton;
		[Export] public NodePath MissingRequirement;

		public override ArtSpec GetArt() => AssetConfigs.SCREEN_BUILDING;

		private IGameObject _root;
		private TechTreeVisualLayout<BuildingSpecId> _tree;
		private GameSpec _specs;
		private bool _isDragging;
		private Vector2 _dragStartPos;
		private TechTreeItemWidget _selected;
		private Label _buildTime;
		private Label _desc;
		private Label _name;
		private GridContainer _costsGrid;
		private ScreenHeader _header;
		private Button _buildButton;
		private Control _requirement;

		public override void OnBuild()
		{
			_requirement = GetNode<Control>(MissingRequirement);
			_buildButton = GetNode<Button>(BuildButton);
			_header = GetNode<ScreenHeader>(HeaderPath);
			_buildTime = GetNode<Label>(BuildTimeLabel);
			_desc = GetNode<Label>(DescLabel);
			_name = GetNode<Label>(NameLabel);
			_costsGrid = GetNode<GridContainer>(CostsGrid);
			_header.SetData("Building Construction", () => { 
				ClientServices.Ui.Destroy<BuildingScreen>(); 
			});
			_root = new GodotGameObject(GetNode<TextureRect>(Bg));
			_specs = ClientServices.GameSpecs;
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
			container.OnClick = OnSelect;
			return container;
		}

		private Control SetRequirement(Control req, string text, ArtSpec icon)
		{
			req.Visible = true;
			var lbl = req.GetNode<Label>("ReqDesc");
			var img = req.GetNode<TextureRect>("ReqImage");
			ClientServices.Assets.LoadGetTexture(icon).Then(tex => { img.Texture = tex; });
			lbl.Text = text;
			return req;
		}

		private void OnSelect(TechTreeItemWidget widget)
		{
			if(_selected != null)
			{
				_selected.SetBorder(HtmlColors.White);
			}
			widget.SetBorder(HtmlColors.LightGreen);
			_selected = widget;
			var id = (BuildingSpecId)widget.Item;
			var buildingSpec = _specs.Buildings[id];
			var constructionSpec = _specs.BuildingConstructions[id];
			var buildStatus = ClientServices.LocalPlayer.EntityLogic.CheckTechTree(id);

			_buildButton.Disabled = !buildStatus.IsAvailable;
			_requirement.Visible = !buildStatus.IsAvailable;

			if (!buildStatus.IsAvailable)
			{
				var blocker = _specs.Buildings[buildStatus.BlockedBy.Value];
				var blockerConstruction = _specs.BuildingConstructions[buildStatus.BlockedBy.Value];
				SetRequirement(_requirement, "Needs "+ blocker.Name, blockerConstruction.Icon);
			} else {
				_requirement.Visible = false;
			}

			constructionSpec.Costs.ForEach(cost => { }); // TODO:
			_name.Text = buildingSpec.Name;
			_desc.Text = buildingSpec.Description;
			_buildTime.Text = constructionSpec.TimeToBuildSeconds+" Seconds";
			foreach(var c in _costsGrid.GetChildren())
			{
				_costsGrid.RemoveChild(c);
				c.QueueFree();
			}
			foreach (var cost in constructionSpec.Costs)
			{
				ClientServices.Ui.CreateWidget<ItemStackWidget>().Then(item =>
				{
					item.SetData(cost);
					_costsGrid.AddChild(item);
				});
			}
		}

		public override void _Input(InputEvent @event)
		{
			if(_tree == null) return;
			if (@event is InputEventMouseButton mouseEvent)
			{
				if (mouseEvent.ButtonIndex == MouseButton.Left)
				{
					if (mouseEvent.Pressed)
					{
						_isDragging = true;
						_dragStartPos = mouseEvent.Position;
					}
					else _isDragging = false;
				}
			}
			else if (@event is InputEventMouseMotion motionEvent && _isDragging)
			{
				Vector2 delta = _dragStartPos - motionEvent.Position;
				_tree.ScrollContainer.ScrollHorizontal += (int)Math.Round(delta.X);
				_tree.ScrollContainer.ScrollVertical += (int)Math.Round(delta.Y);
				_dragStartPos = motionEvent.Position;
			}
		}
	}
}
