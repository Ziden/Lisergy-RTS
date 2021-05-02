using Assets.Code.World;
using Game;
using Game.TacticsBattle;
using System.Linq;

namespace Assets.Code.MapEditor
{
    public class ClientTacticsMap : TacticsMap
    {
        public ClientTacticsMap(int x, int y): base(x, y)
        {
            foreach (var c in this.AllChunks())
                foreach (var t in c.AllTiles())
                    t.TileId = 0;
            Log.Debug("Initialized Tactics Map");
        }

        public override Tile GenerateTile(Chunk c, int tx, int ty)
        {
            Log.Debug($"Creating {this}");
            var tile = new ClientTacticsTile(c, tx, ty);
            return tile;
        }

        public override Chunk GenerateChunk(int chunkX, int chunkY)
        {
            return new ClientChunk(this, chunkX, chunkY);
        }

        public override Tile GetTile(int tileX, int tileY)
        {
            if (!ValidCoords(tileX, tileY))
            {
                StackLog.Debug($"Invalid coords {tileX}-{tileY}");
                return null;
            }
            var d = this.AllChunks().ToList();
            return base.GetTile(tileX, tileY);
        }
    }
}
