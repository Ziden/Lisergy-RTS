using Game;
using Game.Events;
using Game.World;
using GameData;
using GameDataTest;
using LisergyServer.Core;
using System.Collections.Generic;
using System.Linq;

namespace ServerTests
{
    public class TestServerPlayer : ServerPlayer
    {
        public static string TEST_ID = "test_player_id";

        public delegate void ReceiveEventHandler(GameEvent ev);
        public event ReceiveEventHandler OnReceiveEvent;
        public List<GameEvent> ReceivedEvents = new List<GameEvent>();

        public bool IsOnline { get; set; }

        public TestServerPlayer() : base(null, null)
        {

        }

        public override void Send<EventType>(EventType ev)
        {
            ev.Sender = this;
            OnReceiveEvent?.Invoke(ev);
            ReceivedEvents.Add(ev);
        }

        public void SendEventToServer(ClientEvent ev) {
            EventEmitter.CallEventFromBytes(this, Serialization.FromEvent(ev));
        }

        public List<T> ReceivedEventsOfType<T>() where T : ServerEvent
        {
            return ReceivedEvents.Where(e => e.GetType().IsAssignableFrom(typeof(T))).Select(e => e as T).ToList();
        }

        public override bool Online()
        {
            return this.IsOnline;
        }

        public override string ToString()
        {
            return $"<TestPlayer id={UserID.ToString()}>";
        }
    }

    public class TestGame : StrategyGame
    {
        private bool _registered = false;
        public TestGame(GameWorld world=null, bool createPlayer=true) : base(GetTestConfiguration(), GetTestSpecs(), world == null ? new GameWorld() : world)
        {
            this.RegisterEventListeners();
            if (!_registered)
            {
                NetworkEvents.OnTileVisible += ev => ReceiveEvent(ev);
                NetworkEvents.OnPlayerAuth += ev => ReceiveEvent(ev);
                NetworkEvents.OnSpecResponse += ev => ReceiveEvent(ev);
                NetworkEvents.OnJoinWorld += ev => ReceiveEvent(ev);
                NetworkEvents.OnEntityVisible += ev => ReceiveEvent(ev);
                _registered = true;
            }
            Serialization.LoadSerializers();
            this.World.CreateWorld(4);
            this.World.ChunkMap.SetFlag(0, 0, ChunkFlag.NEWBIE_CHUNK);
            if(createPlayer)
                CreatePlayer();
        }

        public void HandleClientEvent<T>(PlayerEntity sender, T ev) where T : ClientEvent
        {
            EventEmitter.CallEventFromBytes(sender, Serialization.FromEvent<T>(ev));
        }

        public void CreatePlayer()
        {
            var player = new TestServerPlayer();
            player.OnReceiveEvent += ev => ReceiveEvent(ev);
            player.UserID = TestServerPlayer.TEST_ID;
            this.World.PlaceNewPlayer(player, this.World.GetTile(10, 10));
        }

        public void ReceiveEvent(GameEvent ev)
        {
            ReceivedEvents.Add(ev); 
        }

        public List<GameEvent> ReceivedEvents = new List<GameEvent>();

        public PlayerEntity GetTestPlayer()
        {
            PlayerEntity pl;
            this.World.Players.GetPlayer(TestServerPlayer.TEST_ID, out pl);
            return pl;
        }

        private static GameSpec GetTestSpecs()
        {
            return TestSpecs.Generate();
        }

        public static BuildingSpec RandomBuildingSpec()
        {
            return StrategyGame.Specs.Buildings.Values.RandomElement();
        }

        public override void GenerateMap()
        {
            base.GenerateMap();
        }

        public Tile RandomNotBuiltTile()
        {
            foreach (var tile in World.AllTiles())
                if (tile.Building==null)
                    return tile;
            return null;
        }

        private static GameConfiguration GetTestConfiguration()
        {
            var cfg = new GameConfiguration();
            return cfg;
        }
    }
}
