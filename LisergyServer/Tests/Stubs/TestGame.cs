using Game;
using Game.BlockChain;
using Game.Events;
using System.Collections.Generic;

namespace Tests
{

    public class TestGame : BlockchainGame
    {
        private bool _registered = false;
        public TestGame(bool createPlayer=true) : base(new TestChain())
        {
            this.RegisterEventListeners();
            if (!_registered)
            {
                _registered = true;
            }
            Serialization.LoadSerializers();
            if (createPlayer)
                CreatePlayer();
        }

        public void HandleClientEvent<T>(PlayerEntity sender, T ev) where T : ClientEvent
        {
            this.NetworkEvents.RunCallbacks(sender, Serialization.FromEventRaw(ev));
        }

        public TestServerPlayer CreatePlayer()
        {
            var player = new TestServerPlayer();
            player.OnReceiveEvent += ev => ReceiveEvent(ev);
            player.UserID = "test_player_id";
            this.Chain.UpdatePlayer(player);
            return player;
        }

        public void ReceiveEvent(BaseEvent ev)
        {
            ReceivedEvents.Add(ev);
        }

        public List<BaseEvent> ReceivedEvents = new List<BaseEvent>();

        public TestServerPlayer GetTestPlayer()
        {
            return (TestServerPlayer)this.Chain.GetPlayer("test_player_id");
        }
    }
}