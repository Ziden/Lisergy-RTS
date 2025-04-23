using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Systems.Tile;
using Game.Tile;

namespace Game.Systems.FogOfWar
{
	public class TileVisibilityChangedEvent : IGameEvent
	{
		public IEntity Explorer;
		public TileModel Tile;
		public bool Visible;

		public override string ToString()
		{
			return $"<VisChangeEv {Tile.Get<TileDataComponent>().Position} Visible: {Visible} to {Explorer}";
		}
	}
}