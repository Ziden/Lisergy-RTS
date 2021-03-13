namespace Game.Battle
{
    public class ActionLog
    {
        public BattleUnit Attacker;

        public ActionLog(BattleUnit atk)
        {
            this.Attacker = atk;
        }
    }
}
