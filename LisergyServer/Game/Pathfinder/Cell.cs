using Game.World;
using Priority_Queue;

namespace AStar
{
    public class Cell : FastPriorityQueueNode
    {
        public bool Blocked;
        public bool Closed;
        public double F;

        public double G;
        public double H;

        public TileVector Location;

        public Cell Parent;

        public int QueueIndex;

        public Cell(TileVector location) => Location = location;

        public override string ToString() => $"[{Location.X},{Location.Y}]";
    }

}