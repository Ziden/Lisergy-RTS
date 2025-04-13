using ClientSDK;
using Game.World;
using GameData;
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
		private IGameObject _buildingSelector;

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
                ClientServices.State.PlacingBuilding.Value = default;
                ClientServices.Ui.Close<ConfirmBuildingDialog>();
			};
			_cancelButton.ButtonDown += () =>
			{
				ClientServices.State.PlacingBuilding.Value = default;
                ClientServices.Ui.Close<ConfirmBuildingDialog>();
            };
		}

        private void OnPlacingBuilding(BuildingSpecId id)
        {
            if (id == default)
            {
                if (_buildingSelector != null)
                {
                    _buildingSelector.Destroy();
                    _buildingSelector = null;
                }
                return;
            }
            var spec = ClientServices.GameSpecs.Buildings[id];
            if (_buildingSelector != null)
            {
                _buildingSelector.Destroy();
                _buildingSelector = null;
            }

            ClientServices.Assets.LoadGetArt(spec.Art).Then(art =>
            {
                _buildingSelector = art;
                ClientServices.Assets.AddToScene(art);
                MoveBuildingSelector(ClientServices.State.SelectedTile.Value.Position);
            });
        }

        private void MoveBuildingSelector(Location tile)
        {
            if (_buildingSelector == null) return;
            _buildingSelector.Location = tile;
        }

        public override void OnOpen()
        {
			ClientServices.State.PlacingBuilding.OnChanged += OnPlacingBuilding;
        }

		public override void OnClose()
		{
			ClientServices.State.PlacingBuilding.OnChanged -= OnPlacingBuilding;
			if (_buildingSelector != null)
			{
                _buildingSelector.Destroy();
                _buildingSelector = null;
            }
        }

        public void SetData(BuildingSpecId id, TileSpecId tile)
		{
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
}
