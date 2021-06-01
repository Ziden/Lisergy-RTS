using GameData;
using System;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class GameSpecResponse : ServerEvent
    {
        public GameSpec Spec;
        public int WorldX;
        public int WorldY;

        public GameSpecResponse(StrategyGame game)
        {
            this.Spec = game.GameSpec;
            this.WorldX = game.World.SizeX;
            this.WorldY = game.World.SizeY;
        }
    }
}
