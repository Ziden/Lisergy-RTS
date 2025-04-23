using System.Collections.Generic;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Systems.Building;
using LisergyGodotClient.Data;
using LisergyGodotClient.Src;
using LisergyGodotClient.Src.Systems;

namespace LisergyGodotClient.Systems.Building;

public class ConstructionProgressPrediction : IAutoRegisterListener
{
	private Dictionary<GameId, TimeBlockTask> _predictionTasks = new();
	
	public void OnRegister()
	{
		ClientServices.ServerSdk.Server.Entities.OnComponentAdded<ConstructionSiteComponent>(OnStartedBuilding);
		ClientServices.ServerSdk.Server.Entities.OnComponentRemoved<ConstructionSiteComponent>(OnStoppedBuilding);
		ClientServices.ServerSdk.Server.Entities.OnComponentModified<ConstructionSiteComponent>(OnBuildingUpdated);
	}

	private void OnBuildingUpdated(IEntity e, ConstructionSiteComponent oldV, ConstructionSiteComponent newV)
	{
		if (oldV.BuildingWorkPrediction != null && newV.BuildingWorkPrediction == null)
		{
			OnStoppedBuilding(e, oldV);
		} else if (oldV.BuildingWorkPrediction == null && newV.BuildingWorkPrediction != null)
		{
			OnStartedBuilding(e, newV);
		}
	}

	private void OnStoppedBuilding(IEntity building, ConstructionSiteComponent arg2)
	{
		if (_predictionTasks.Remove(building.EntityId, out var task))
		{
			task.Dispose();
		}
	}

	private void OnStartedBuilding(IEntity building, ConstructionSiteComponent c)
	{
		if (!building.IsVisible()) return;
		if (!building.Logic.Building.IsBeingBuilt) return;
		var view = building.GetView<PlayerBuildingView>();
		_predictionTasks[building.EntityId] = c.BuildingWorkPrediction.OnFinish(() =>
		{
			_ = view.ReRender();
		});
	}
}