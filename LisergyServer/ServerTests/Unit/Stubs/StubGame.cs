using BattleServer;
using Game;
using Game.Battles;
using Game.Events;
using Game.Listeners;
using Game.World;
using GameData;
using GameDataTest;
using LisergyServer.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ServerTests
{

    public class TestGame : StrategyGame
    {
        private bool _registered = false;

        public BattleService BattleService { get; private set; }
        public WorldService WorldService { get; private set; }
        public CourseService CourseService { get; private set; }


        private static GameWorld GetTestWorld(GameWorld source = null)
        {
            if(source != null)
            {
                return source;
            }
            /*
            if(TestWorld == null)
            {
                TestWorld = new GameWorld(4, 20, 20);
            } else
            {
                DeltaTracker.Clear();
                TestWorld.FreeMap();
            }
            return TestWorld;
            */
            var t = new Stopwatch();
            t.Start();
            var w = new GameWorld(4, 20, 20);
            t.Stop();
            Console.WriteLine("World Gen Time: " + t.ElapsedMilliseconds);
            return w;
        }



        public TestGame(GameWorld world = null, bool createPlayer = true) : base(GetTestSpecs(), GetTestWorld(world))
        {

            var t = new Stopwatch();
            t.Start();
           
          
            if (!_registered)
            {
                _registered = true;
            }
            Serialization.LoadSerializers();

            BattleService = new BattleService(this);
            WorldService = new WorldService(this);
            CourseService = new CourseService(this);

            this.World.Map.SetFlag(0, 0, ChunkFlag.NEWBIE_CHUNK);
            if (createPlayer)
                CreatePlayer();
            t.Stop();
            Console.WriteLine("Test Game Time: " + t.ElapsedMilliseconds);

        }

        public void HandleClientEvent<T>(PlayerEntity sender, T ev) where T : ClientEvent
        {
            this.NetworkEvents.RunCallbacks(sender, Serialization.FromEventRaw(ev));
        }

        public TestServerPlayer CreatePlayer(int x = 10, int y = 10)
        {
            var player = new TestServerPlayer();
            player.OnReceiveEvent += ev => ReceiveEvent(ev);
            player.UserID = TestServerPlayer.TEST_ID;
            this.World.PlaceNewPlayer(player, this.World.GetTile(x, y));
            return player;
        }

        public void ReceiveEvent(BaseEvent ev)
        {
            ReceivedEvents.Add(ev);
        }

        public List<BaseEvent> ReceivedEvents = new List<BaseEvent>();

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


        public Tile RandomNotBuiltTile()
        {
            foreach (var tile in World.AllTiles())
                if (tile.StaticEntity == null)
                    return tile;
            return null;
        }
    }

}
