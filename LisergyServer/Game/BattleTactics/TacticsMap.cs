
namespace Game.TacticsBattle
{
    public class TacticsMap : ChunkMap
    {
        public TacticsMap(int sizeX, int sizeY): base(sizeX, sizeY)
        {
            this.GenerateTiles(sizeX, sizeY);
        }

        public TacticsTile GetTacticsTile(int x, int y)
        {
            return (TacticsTile)GetTile(x, y);
        }

        public override Tile GenerateTile(Chunk c, int tx, int ty)
        {
            return new TacticsTile(c, tx, ty);
        }
    }
}
