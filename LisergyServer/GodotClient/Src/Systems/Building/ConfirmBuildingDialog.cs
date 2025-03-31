using GameData.Specs;
using Godot;
using GodotClient.Services;
using LisergyGodotClient.Src.Systems.Tiles.UI;

namespace LisergyGodotClient.Src.Systems.Building
{
    public partial class ConfirmBuildingDialog : GameUi
    {
        public NodePath ConfirmButton;
        public NodePath CancelButton;
        public NodePath BuildingItem;
        public NodePath TileItem;
        public NodePath TileName;
        public NodePath BuildingName;

        public override ArtSpec GetArt() => AssetConfigs.DIALOG_CONFIRM_BUILDING;
        
        private Button _confirmButton;
        private Button _cancelButton;
        private ItemStackWidget _building;
        private ItemStackWidget _tile;
        public Label _tileName;
        public Label _buildingName;

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
                var spec = ClientServices.State.PlacingBuilding.Value;
                if (spec != null)
                {
                    ClientServices.State.PlacingBuilding.Value = default;
                }
                ClientServices.Ui.Close<ConfirmBuildingDialog>();
            };
            _cancelButton.ButtonDown += () =>
            {
                ClientServices.State.PlacingBuilding.Value = default;
            };
        }
    }
}
