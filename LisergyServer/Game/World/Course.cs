using System;
using System.Collections.Generic;

namespace Game.World
{
    public class Course
    {
        public string ID;
        public string UserID;
        public int PartyIndex;
        List<Tile> Path;
        public long StartTime;
    }
}
