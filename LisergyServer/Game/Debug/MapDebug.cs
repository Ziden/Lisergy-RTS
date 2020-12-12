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
            for (var x = 0; x < w.GetSize(); x++)
            {
                for (var y = 0; y < w.GetSize(); y++)
                {
                    var tile = w.GetTile(x, y);
                    if (tile == null)
                        Console.Write("N");
                    else if (tile.BuildingID != 0)
                        Console.Write("^");
                    else if (tile.TerrainData.HasFlag(TerrainData.FOREST))
                        Console.Write("|");
                    else if (tile.TerrainData.HasFlag(TerrainData.BUSHES))
                        Console.Write("B");
                    else if (tile.TerrainData.HasFlag(TerrainData.MOUNTAIN))
                        Console.Write("A");
                    else
                        Console.Write("-");
                }
                Console.Write("\n");
            }
        }
    }
}
