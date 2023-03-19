using BattleServer;
using Game;
using Game.Battles;
using Game.ECS;
using Game.Events;
using Game.Events.GameEvents;
using Game.Listeners;
using Game.World;
using Game.World.Components;
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

        public BattleService BattleService { get; private set; }
        public WorldService WorldService { get; private set; }
        public CourseService CourseService { get; private set; }

        private static GameWorld TestWorld;

        private static GameWorld GetTestWorld(GameWorld source = null)
        {
            DeltaTracker.Clear();
            if(source != null)
            {
                return source;
            }
            UnmanagedMemory.FlagMemoryToBeReused();
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
            return new GameWorld(4, 20, 20);
        }



        public TestGame(GameWorld world = null, bool createPlayer = true) : base(GetTestSpecs(), GetTestWorld(world))
        {
            
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
        }

        public void HandleClientEvent<T>(PlayerEntity sender, T ev) where T : ClientPacket
        {
            this.NetworkEvents.RunCallbacks(sender, Serialization.FromEventRaw(ev));
            DeltaTracker.SendDeltaPackets(sender);
        }

        public TestServerPlayer CreatePlayer(int x = 10, int y = 10)
        {
            var player = new TestServerPlayer(); 
            player.OnReceiveEvent += ev => ReceiveEvent(ev);
            player.UserID = TestServerPlayer.TEST_ID;
            var tile = this.World.GetTile(x, y);
            this.World.PlaceNewPlayer(player, this.World.GetTile(x, y));
            DeltaTracker.SendDeltaPackets(player);
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
            var tiles = World.AllTiles();
            foreach (var tile in tiles)
                if (tile.Components.Get<TileHabitants>().StaticEntity == null)
                    return tile;
            throw new System.Exception("No unbuilt tile");
        }
    }

}
