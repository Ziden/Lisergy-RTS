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

        public List<GameEvent> SentEvents = new List<GameEvent>();
        public bool IsOnline { get; set; }

        public TestServerPlayer() : base(null, null)
        {

        }

        public override void Send<EventType>(EventType ev)
        {
            SentEvents.Add(ev);
        }

        public override bool Online()
        {
            return this.IsOnline;
        }
    }

    public class TestGame : StrategyGame
    {
        private bool _registered = false;
        public TestGame() : base(GetTestConfiguration(), GetTestSpecs(), new GameWorld())
        {
            if(!_registered)
            {
                EventSink.OnTileVisible += ev => ReceiveEvent(ev);
                EventSink.OnPlayerAuth += ev => ReceiveEvent(ev);
                EventSink.OnSpecResponse += ev => ReceiveEvent(ev);
                EventSink.OnJoinWorld += ev => ReceiveEvent(ev);
                EventSink.OnPartyVisible += ev => ReceiveEvent(ev);
                _registered = true;
            }
           
            LoadMap();
            var player = new TestServerPlayer();
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
