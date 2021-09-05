using Game;
using Game.Battles;
using Game.Events.Bus;
using System.Collections.Generic;
using System.Linq;

namespace BattleService
{
    public class BattlePacketListener : IEventListener
    {
        public BlockchainGame Game;

        private Dictionary<string, TurnBattle> _battlesHappening = new Dictionary<string, TurnBattle>();

        public void Wipe()
        {
            _battlesHappening.Clear();
        }

        public List<TurnBattle> GetBattles()
        {
            return _battlesHappening.Values.ToList();
        }

        public BattlePacketListener(BlockchainGame game)
        {
            Game = game;
        }
    }
}
