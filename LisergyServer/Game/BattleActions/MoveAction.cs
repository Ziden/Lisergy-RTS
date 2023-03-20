using System;

namespace Game.Battles.Actions
{
    [Serializable]
    public class MoveAction : BattleAction
    {
        public int TileX;
        public int TileY;

        public MoveAction(TurnBattle battle, BattleUnit atk, int tileX, int tileY) : base(battle, atk)
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
