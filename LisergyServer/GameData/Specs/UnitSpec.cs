using Game.Entity;
using System;

namespace GameData.Specs
{
    [Serializable]
    public class UnitSpec
    {
        public string Name;
        public ushort UnitSpecID;
        public byte LOS;
        public ArtSpec Art;
        public ArtSpec FaceArt;

        public UnitStats Stats;
    }
}
