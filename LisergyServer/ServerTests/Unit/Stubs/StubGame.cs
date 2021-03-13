using Game;
using Game.Battle;
using Game.Events;
using Game.World;
using GameData;
using GameDataTest;
using LisergyServer.Core;
using System.Collections.Generic;
using System.Linq;

namespace ServerTests
{

    public class TestGame : StrategyGame
    {
        private bool _registered = false;
        public TestGame(GameWorld world = null, bool createPlayer = true) : base(GetTestConfiguration(), GetTestSpecs(), world == null ? new GameWorld() : world)
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
            if (createPlayer)
                CreatePlayer();
        }

        public void HandleClientEvent<T>(PlayerEntity sender, T ev) where T : ClientEvent
        {
            EventEmitter.CallEventFromBytes(sender, Serialization.FromEvent<T>(ev));
        }

        public TestServerPlayer CreatePlayer(int x = 10, int y = 10)
        {
            var player = new TestServerPlayer();
            player.OnReceiveEvent += ev => ReceiveEvent(ev);
            player.UserID = TestServerPlayer.TEST_ID;
            this.World.PlaceNewPlayer(player, this.World.GetTile(x, y));
            return player;
        }

        public void ReceiveEvent(GameEvent ev)
        {
            ReceivedEvents.Add(ev);
        }

        public List<GameEvent> ReceivedEvents = new List<GameEvent>();

        public TestServerPlayer GetTestPlayer()
        {
            PlayerEntity pl;
            this.World.Players.GetPlayer(TestServerPlayer.TEST_ID, out pl);
            return (TestServerPlayer)pl;
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
                if (tile.Building == null)
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
