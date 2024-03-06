using Game.Engine.DataTypes;
using Game.World;

namespace Game.Engine.Pathfinder
{
    public class Cell : FastPriorityQueueNode
    {
        public bool Blocked;
        public bool Closed;
        public double F;

        public double G;
        public double H;

        public Location Location;

        public Cell Parent;

        public int QueueIndex;

        public Cell(Location location) => Location = location;

        public override string ToString() => $"[{Location.X},{Location.Y}]";
    }

}