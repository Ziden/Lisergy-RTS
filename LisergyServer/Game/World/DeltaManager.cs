using Game.Events.ServerEvents;
using System;
using System.Collections.Generic;

namespace Game.World
{
    public class DeltaManager
    {
        
        private static List<Tuple<WorldEntity, Tile>> _tileVisibleUpdates = new List<Tuple<WorldEntity, Tile>>();

        public static void RecordTileVisible(WorldEntity viewer, Tile t)
        {
            _tileVisibleUpdates.Add(new Tuple<WorldEntity, Tile>(viewer, t));
        }

        public static void ProccessDeltas()
        {
            var tileUpdates = new TileVisibleEvent();
            foreach(var tuple in _tileVisibleUpdates)
            {

            }
        }

    }
}
