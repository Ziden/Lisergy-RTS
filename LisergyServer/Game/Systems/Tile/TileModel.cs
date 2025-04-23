using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Entities;
using Game.Systems.Tile;
using Game.World;
using GameData;

namespace Game.Tile
{
	public class TileModel
	{
		public TileModel(Chunk c, IEntity tileEntity)
		{
			Chunk = c;
			Entity = tileEntity;
		}

		public IEntity Entity { get; protected set; }
		public Chunk Chunk { get; protected set; }
		public EntityType EntityType => EntityType.Tile;

		public ref readonly GameId EntityId => ref Entity.EntityId;
		public ComponentSet Components => Entity.Components;
		public ref readonly byte SpecId => ref Components.Get<TileDataComponent>().TileId;
		public ref readonly Location Position => ref Get<TileDataComponent>().Position;
		public ref readonly ushort Y => ref Position.Y;
		public ref readonly ushort X => ref Position.X;
		public IGame Game => Chunk.World.Game;
		public TileSpec Spec => Game.Specs.Tiles[SpecId];
		public bool HasHarvestSpot => HarvestPointSpec != null;

		public ResourceHarvestPointSpec HarvestPointSpec => Spec.ResourceSpotSpecId.HasValue
			? Game.Specs.HarvestPoints[Spec.ResourceSpotSpecId.Value]
			: null;

		public EntityLogic Logic => Game.Logic.GetEntityLogic(Entity);

		public override string ToString()
		{
			return $"<Tile {Position} Id={EntityId}>";
		}

		public T Get<T>() where T : IComponent
		{
			return Components.Get<T>();
		}

		public void Save<T>(in T c) where T : IComponent
		{
			Components.Save(c);
		}
	}
}