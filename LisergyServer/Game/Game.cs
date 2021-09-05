using BattleService;
using Game.BlockChain;
using Game.Events.Bus;
using Game.InfiniteDungeon;
using Game.Listeners;

namespace Game
{
    public class BlockchainGame
    { 
        public EventBus NetworkEvents { get; private set; }

        public IChain Chain { get; private set; }

        public BlockchainGame(IChain blockChain)
        {
            NetworkEvents = new EventBus();
            Chain = blockChain;
        }

        public virtual void RegisterEventListeners()
        {
            NetworkEvents.RegisterListener(new InfiniteDungeonListener(this));
            NetworkEvents.RegisterListener(new BattlePacketListener(this));
            NetworkEvents.RegisterListener(new TownPacketListener(this));
        }

        public ListenerType GetListener<ListenerType>() where ListenerType: IEventListener
        {
            return (ListenerType)NetworkEvents.GetListener(typeof(ListenerType));
        }

        public void ClearEventListeners()
        {
            NetworkEvents.Clear();
        }

        public virtual void GenerateMap()
        {
            
        }
    }
}
