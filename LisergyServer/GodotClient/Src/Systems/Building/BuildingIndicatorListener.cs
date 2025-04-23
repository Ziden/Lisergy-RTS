using ClientSDK;
using Game.Tile;
using Game.World;
using GameData;
using LisergyGodotClient.Data;

namespace LisergyGodotClient.Src.Systems.Building;

public class BuildingIndicatorListener : IAutoRegisterListener
{
	private IGameObject _buildingSelector;
	
	public void OnRegister()
	{
		ClientServices.State.PlacingBuilding.OnChanged += OnPlacingBuilding;
		ClientServices.State.SelectedTile.OnChanged += OnSelectTile;
	}
	
	private void OnSelectTile(TileModel obj)
	{
		MoveBuildingSelector(obj.Position);
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
}