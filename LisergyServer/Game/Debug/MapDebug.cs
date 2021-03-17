using Game.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Debug
{
    public static class MapDebug
    {
        public static void PrintAscii(GameWorld w)
        {
            /*
            for (var x = 0; x < w.SizeX; x++)
            {
                for (var y = 0; y < w.SizeY; y++)
                {
                    var tile = w.GetTile(x, y);
                    if (tile == null)
                        Console.Write("N");
                    else if (tile.Building?.SpecID != 0)
                        Console.Write("^");
                    else if (tile.TileId == 0)
                        Console.Write("|");
                    else if (tile.TileId == 1)
                        Console.Write("B");
                    else if (tile.TileId == 2)
                        Console.Write("A");
                    else if (tile.TileId == 3)
                        Console.Write("U");
                    else
                        Console.Write("-");
                }
                Console.Write("\n");
            }
            */
        }
    }
}
