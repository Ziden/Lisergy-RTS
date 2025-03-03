using Game.Engine.Pathfinder;

namespace Game.World
{
    /// <summary>
    /// Used for pathfinding.
    /// </summary>
    public class PathfindingChunkMap : IPathfinderGridProvider
    {
        public Cell[,] CellArray;
        private Location _size;
        private IChunkMap _chunkMap;

        public PathfindingChunkMap(IChunkMap chunkMap)
        {
            _chunkMap = chunkMap;
            _size = new Location(SizeX, SizeY);
            CellArray = new Cell[SizeX, SizeY];
        }

        public void Reset()
        {
            CellArray = new Cell[SizeX, SizeY];
        }

        public int SizeX { get => _chunkMap.TilemapDimensions.x; }
        public int SizeY { get => _chunkMap.TilemapDimensions.y; }
        public Location Size => _size;

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
