using Game;
using Game.Battles;
using Game.Events;

namespace Game.Listeners
{
    public class BattleFinishListener : EventListener
    {
        private GameWorld _world;

        public BattleFinishListener(GameWorld world)
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

        public static BattleResultCompleteEvent HandleBattle(BattleStartEvent ev)
        {
            /*
            var battle = new TurnBattle(Guidev.BattleID, ev.Attacker, ev.Defender);
            var result = battle.Run();
            var resultEvent = new BattleResultCompleteEvent(result);
            resultEvent.BattleID = ev.BattleID;
            return resultEvent;
            */
            return null; 
        }

    }
}
