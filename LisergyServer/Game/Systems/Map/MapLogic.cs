using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Entities;
using Game.Systems.Building;
using Game.Systems.DeltaTracker;
using Game.Systems.Tile;
using Game.Tile;

namespace Game.Systems.Map
{
	public class MapLogic : BaseEntityLogic<MapPlaceableComponent>
	{
		public TileModel GetTile()
		{
			if (CurrentEntity.EntityType == EntityType.Tile)
				return Game.World.GetTile(CurrentEntity.Components.Get<TileDataComponent>().Position);
			if (!CurrentEntity.Components.TryGet<MapPlacementComponent>(out var c)) return null;
			return Game.World.GetTile(c.Position);
		}

		public void RemoveEntityFromTile(IEntity e)
		{
			if (CurrentEntity.Components.TryGet<TileHabitantsComponent>(out var habitants))
			{
				if (e.Components.Has<ConstructionComponent>())
					habitants.Building = null;
				else
					habitants.EntitiesIn.Remove(e);
				CurrentEntity.Save(habitants);
			}
		}

		public void SetEntityOnTile(IEntity steppingOn)
		{
			if (!CurrentEntity.Components.TryGet<TileHabitantsComponent>(out var habitants))
				habitants = new TileHabitantsComponent();
			if (steppingOn.Components.Has<ConstructionComponent>())
				habitants.Building = steppingOn;
			else
				habitants.EntitiesIn.Add(steppingOn);
			CurrentEntity.Save(habitants);
		}

		public void SetPosition(TileModel newTile)
		{
			var moving = CurrentEntity;
			var wasPlaced = moving.Components.TryGet<MapPlacementComponent>(out var placement);
			var hasPreviousTile = moving.Components.TryGet<PreviousMapPlacementComponent>(out var previous);

			placement = placement ?? new MapPlacementComponent();
			previous = previous ?? new PreviousMapPlacementComponent();

			var previousTile = wasPlaced ? Game.World.GetTile(placement.Position) : null; // component.Tile;

			if (previousTile == null || newTile == null) moving.Logic.DeltaCompression.SetFlag(DeltaFlag.CREATED);
			if (newTile != null)
			{
				placement.Position = newTile.Position;
				if (wasPlaced)
				{
					previous.Position = previousTile.Position;
					moving.Save(previous);
				}

				moving.Save(placement);
			}
			else
			{
				moving.Components.Remove<MapPlacementComponent>();
				if (hasPreviousTile) moving.Components.Remove<PreviousMapPlacementComponent>();
				if (previousTile != null)
				{
					var ev = EventPool<EntityRemovedFromMapEvent>.Get();
					ev.Entity = moving;
					ev.Tile = previousTile;
					moving.Components.CallEvent(ev);
					EventPool<EntityRemovedFromMapEvent>.Return(ev);
				}
			}

			Game.Log.Debug($"MapLogic - Set {moving} position to {newTile?.Position}");
		}
	}
}