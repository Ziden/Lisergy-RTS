using System;

namespace Game.Battles.Actions
{
    [Serializable]
    public class MoveAction : BattleAction
    {
        public int TileX;
        public int TileY;

        public MoveAction(BattleUnit atk, int tileX, int tileY): base(atk)
        {
            TileX = tileX;
            TileY = tileY;
        }

        public override string ToString()
        {
            return $"<Move {Unit} to {TileX}-{TileY}>";
        }
    }
}
