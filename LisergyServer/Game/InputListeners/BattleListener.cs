using Game;
using Game.Battles;
using Game.Events;

namespace Game.Listeners
{
    public class BattleListener : EventListener
    {
        private GameWorld _world;

        public BattleListener(GameWorld world)
        {
            this._world = world;
        }

        public override void Register()
        {
            NetworkEvents.OnBattleResult += BattleFinished;
            Log.Debug("Battle Result Listener Registered");
        }

        public override void Unregister()
        {
            
        }

        public void BattleFinished(BattleResultCompleteEvent ev)
        {
            // copy values to real units
        }

        public static BattleResultCompleteEvent HandleBattle(BattleStartCompleteEvent ev)
        {
            var battle = new TurnBattle(ev.Attacker, ev.Defender);
            var result = battle.Run();
            var resultEvent = new BattleResultCompleteEvent(result);
            resultEvent.BattleID = ev.BattleID;
            return resultEvent;
        }

    }
}
