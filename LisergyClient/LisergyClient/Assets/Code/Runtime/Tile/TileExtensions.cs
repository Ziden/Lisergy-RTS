using Assets.Code.Views;
using Game.Tile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Assets.Code.Tile
{
    public static class TileExtensions
    {
        public static bool IsVisible(this TileEntity tile)
        {
            return tile.EntitiesViewing.Any(e => e.Id == MainBehaviour.LocalPlayer.EntityId);
        }
    }
}
