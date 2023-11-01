using Game.World;

namespace AStar
{
    public interface IPathfinderGridProvider
    {
        TileVector Size { get; }
        Cell this[TileVector position] { get; }
        void Reset();
    }

}