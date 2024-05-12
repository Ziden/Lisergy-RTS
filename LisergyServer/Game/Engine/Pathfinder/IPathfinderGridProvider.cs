using Game.World;

namespace Game.Engine.Pathfinder
{
    public interface IPathfinderGridProvider
    {
        Location Size { get; }
        Cell this[Location position] { get; }
        void Reset();
    }

}