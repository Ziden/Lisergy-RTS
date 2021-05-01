using Game.Battles;

namespace Game.TacticsBattle
{
    public class TacticsTile : Tile
    {
        private byte _z;

        public TacticsTile(Chunk c, int x, int y) : base(c, x, y) { }

        public byte Z { get => _z; set => _z = value; }
     
    }
}
