using Game.Engine.Pathfinder;

namespace Game.World
{
    /// <summary>
    ///     Used for pathfinding.
    /// </summary>
    public class PathfindingChunkMap : IPathfinderGridProvider
	{
		private readonly IChunkMap _chunkMap;
		public Cell[,] CellArray;

		public PathfindingChunkMap(IChunkMap chunkMap)
		{
			_chunkMap = chunkMap;
			Size = new Location(SizeX, SizeY);
			CellArray = new Cell[SizeX, SizeY];
		}

		public int SizeX => _chunkMap.TilemapDimensions.x;
		public int SizeY => _chunkMap.TilemapDimensions.y;

		public void Reset()
		{
			CellArray = new Cell[SizeX, SizeY];
		}

		public Location Size { get; }

		public Cell this[Location position]
		{
			get
			{
				var cell = CellArray[position.X, position.Y];
				if (cell == null)
				{
					cell = new Cell(position);
					cell.Blocked = !_chunkMap.GetTile(position.X, position.Y)?.Logic.Tile.IsPassable() ?? false;
					CellArray[position.X, position.Y] = cell;
				}

				return cell;
			}
		}
	}
}