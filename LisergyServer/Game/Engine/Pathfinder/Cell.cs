using Game.Engine.DataTypes;
using Game.World;

namespace Game.Engine.Pathfinder
{
    public class Cell : FastPriorityQueueNode
    {
        public bool Blocked;
        public bool Closed;
        public float F;

        public float G;
        public float H;

        public Location Location;

        public Cell Parent;

        public int QueueIndex;

        public Cell(Location location) => Location = location;

        public override string ToString() => $"[{Location.X},{Location.Y}]";
    }

}