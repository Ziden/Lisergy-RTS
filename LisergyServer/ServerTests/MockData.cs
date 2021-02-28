using Game;
using Game.Events;
using Game.World;
using Game.Generator;
using GameData;
using GameDataTest;
using LisergyServer.Auth;
using LisergyServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telepathy;

namespace ServerTests
{
    public class TestServerPlayer : ServerPlayer
    {
        public static string TEST_ID = "test_player_id";

        public delegate void ReceiveEventHandler(GameEvent ev);
        public event ReceiveEventHandler OnReceiveEvent;

        public bool IsOnline { get; set; }

        public TestServerPlayer() : base(null, null)
        {

        }

        public override void Send<EventType>(EventType ev)
        {
            ev.Sender = this;
            OnReceiveEvent?.Invoke(ev);
        }

        public override bool Online()
        {
            return this.IsOnline;
        }
    }

    public class TestGame : StrategyGame
    {
        private bool _registered = false;
        public TestGame(GameWorld world=null) : base(GetTestConfiguration(), GetTestSpecs(), world == null ? new GameWorld() : world)
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
            
            this.World.CreateWorld(4);
            this.World.ChunkMap.SetFlag(0, 0, ChunkFlag.NEWBIE_CHUNK);
            var player = new TestServerPlayer();
            player.OnReceiveEvent += ev => ReceiveEvent(ev);
            player.UserID = TestServerPlayer.TEST_ID;
            this.World.PlaceNewPlayer(player, this.World.GetTile(10,10));
           
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
