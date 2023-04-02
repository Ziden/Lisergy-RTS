using Game.Tile;
using UnityEngine;

namespace Assets.Code.Code.Utils
{
    public static class Extensions
    {


        public static Vector3 Position(this TileEntity tile,float y = 0)
        {
            return new Vector3(tile.X, y, tile.Y);
        }
    }
}