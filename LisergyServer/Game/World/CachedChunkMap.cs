using AStar;
using Game.Tile;

namespace Game.World
{
    /// <summary>
    /// Used for pathfinding.
    /// </summary>
    public class CachedChunkMap : IPathfinderGridProvider
    {
        public Cell[,] CellArray;
        private TileVector _size;
        private IChunkMap _chunkMap;

        public CachedChunkMap(IChunkMap chunkMap)
        {
            _chunkMap = chunkMap;
            _size = new TileVector(SizeX, SizeY);
            CellArray = new Cell[SizeX, SizeY];
        }

        public void Reset()
        {
            CellArray = new Cell[SizeX, SizeY];
        }

        public int SizeX { get => _chunkMap.TilemapDimensions.x; }
        public int SizeY { get => _chunkMap.TilemapDimensions.y; }
        public TileVector Size => _size;

        public Cell this[TileVector position]
        {
            get
            {
                var cell = CellArray[position.X, position.Y];
                if (cell == null)
                {
                    cell = new Cell(position);
                    cell.Blocked = !_chunkMap.GetTile(position.X, position.Y).Passable;
                    CellArray[position.X, position.Y] = cell;
                }
                return cell;
            }
        }
    }
}
