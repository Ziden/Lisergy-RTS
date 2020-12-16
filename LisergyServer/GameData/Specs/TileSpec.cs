using GameData.Specs;
using System;
using System.Collections.Generic;

namespace GameData
{
    [Serializable]
    public class TileSpec
    {
        public int ID;
        public List<ArtSpec> Arts;
    }
}
