using Assets.Code.World;
using Game;

namespace Assets.Code.MapEditor
{
    public class ClientTacticsTile : ClientTile
    {
        public ClientTacticsTile(Chunk c, int x, int y) : base(c, x, y)
        {

        }

        public override string GetPrefabFolder()
        {
            return "tacticstiles";
        }

        public override void Decorate()
        {
            var tileBhv = GetGameObject().GetComponent<TileRandomizerBehaviour>();
            tileBhv.CreateTileDecoration(this);
        }
    }
}
