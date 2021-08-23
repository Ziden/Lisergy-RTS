using GameData;
using System;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class GameSpecPacket : ServerEvent
    {
        public GameSpec Spec;
        public int WorldX;
        public int WorldY;

        public GameSpecPacket(StrategyGame game)
        {
            this.Spec = game.GameSpec;
            this.WorldX = game.World.SizeX;
            this.WorldY = game.World.SizeY;
        }
    }
}
