using Game.Entity;
using GameData.buffs;
using System;
using System.Collections.Generic;
using System.Text;

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
