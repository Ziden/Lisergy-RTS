using GameData;
using GameData.Specs;
using Godot;
using GodotClient.Services;
using LisergyGodotClient.Src.Systems.Tiles.UI;
using System;

namespace LisergyGodotClient.Src.Systems.Building
{
    public partial class BuildingInfoWidget : GameUiWidget
    {
        [Export] public NodePath CloseButton;
     
        [Export] public NodePath CostsGrid;
        [Export] public NodePath NameLabel;
        [Export] public NodePath DescLabel;
        [Export] public NodePath BuildTimeLabel;
        [Export] public NodePath BuildButton;
        [Export] public NodePath MissingRequirement;

        private Label _buildTime;
        private Label _desc;
        private Label _name;
        private GridContainer _costsGrid;
        private Button _buildButton;
        private Control _requirement;
        private Control _rightBar;
        private Button _closeButton;

        public Action OnClickBuild;
        public Action OnClickClose;

        public override void OnBuild()
        {
          
            _closeButton = GetNode<Button>(CloseButton);
            _requirement = GetNode<Control>(MissingRequirement);
            _buildButton = GetNode<Button>(BuildButton);
            _buildButton.ButtonDown += () =>
            {
                OnClickBuild?.Invoke();
            };
            _buildTime = GetNode<Label>(BuildTimeLabel);
            _desc = GetNode<Label>(DescLabel);
            _name = GetNode<Label>(NameLabel);
            _costsGrid = GetNode<GridContainer>(CostsGrid);
        
            _closeButton.ButtonDown += () =>
            {
                OnClickClose?.Invoke();
            };
        }

        private Control SetRequirement(Control req, string text, ArtSpec icon)
        {
            req.Visible = true;
            var lbl = req.GetNode<Label>("ReqDesc");
            var img = req.GetNode<TextureRect>("Panel/ReqImage");
            ClientServices.Assets.LoadGetTexture(icon).Then(tex => { img.Texture = tex; });
            lbl.Text = text;
            return req;
        }

        public void SetData(BuildingSpecId id)
        {
            var buildingSpec = ClientServices.GameSpecs.Buildings[id];
            var constructionSpec = ClientServices.GameSpecs.BuildingConstructions[id];
            var buildStatus = ClientServices.LocalPlayer.EntityLogic.CheckTechTree(id);

            _rightBar.Visible = true;
            _buildButton.Disabled = !buildStatus.IsAvailable;
            _requirement.Visible = !buildStatus.IsAvailable;

            if (!buildStatus.IsAvailable)
            {
                var blocker = ClientServices.GameSpecs.Buildings[buildStatus.BlockedBy.Value];
                var blockerConstruction = ClientServices.GameSpecs.BuildingConstructions[buildStatus.BlockedBy.Value];
                SetRequirement(_requirement, "Needs " + blocker.Name, blockerConstruction.Icon);
            }
            else
            {
                _requirement.Visible = false;
            }

            constructionSpec.Costs.ForEach(cost => { }); // TODO:
            _name.Text = buildingSpec.Name;
            _desc.Text = buildingSpec.Description;
            _buildTime.Text = constructionSpec.TimeToBuildSeconds + " Seconds";
            foreach (var c in _costsGrid.GetChildren())
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

        public override ArtSpec GetArt() => AssetConfigs.WIDGET_BUILDING_INFO;
    }
}
