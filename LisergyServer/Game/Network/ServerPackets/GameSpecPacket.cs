using GameData;
using System;

namespace Game.Events.ServerEvents
{
    [Serializable]
    public class GameSpecPacket : ServerPacket
    {
        public GameSpec Spec;
        public int WorldX;
        public int WorldY;

        public GameSpecPacket(GameLogic game)
        {
            this.Spec = GameLogic.Specs;
            this.WorldX = game.World.SizeX;
            this.WorldY = game.World.SizeY;
        }
    }
}
