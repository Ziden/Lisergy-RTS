using Game;
using Game.DataTypes;
using Game.Events;
using Game.Network;
using Game.Services;
using Game.Systems.Player;
using Game.Systems.Tile;
using Game.Tile;
using Game.World;
using GameData;
using GameDataTest;
using LisergyServer.Core;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ServerTests
{
    public class TestGame : GameLogic
    {
        private GameId _testPlayerId = GameId.Generate();

        public BattleService BattleService { get; private set; }
        public WorldService WorldService { get; private set; }
        public CourseService CourseService { get; private set; }

        private static GameWorld TestWorld;

        private GameWorld CreateTestWorld(GameWorld source = null)
        {
            WorldUtils.SetRandomSeed(666);
            DeltaTracker.Clear();
            if (source != null)
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
            var w = new GameWorld(4, 20, 20);
            SetWorld(w);
            w.CreateMap();
            return w;
        }

        public TestGame(GameWorld world = null, bool createPlayer = true) : base(GetTestSpecs())
        {
            CreateTestWorld();
            Serialization.LoadSerializers();
            Events.Clear();
            NetworkPackets.Clear();
            BattleService = new BattleService(this);
            WorldService = new WorldService(this);
            CourseService = new CourseService(this);
            this.World.Map.SetFlag(0, 0, ChunkFlag.NEWBIE_CHUNK);
            if (createPlayer)
                CreatePlayer();
        }

        public void HandleClientEvent<T>(PlayerEntity sender, T ev) where T : ClientPacket
        {
            BaseEvent deserialized = Serialization.ToEventRaw(Serialization.FromEventRaw(ev));
            deserialized.Sender = sender;
            NetworkPackets.Call(deserialized);
            DeltaTracker.SendDeltaPackets(sender);
        }

        public TestServerPlayer CreatePlayer(int x = 10, int y = 10)
        {
            var player = new TestServerPlayer(this);
            player.OnReceiveEvent += ev => ReceiveEvent(ev);
            _testPlayerId = player.UserID;
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
            this.World.Players.GetPlayer(_testPlayerId, out pl);
            return (TestServerPlayer)pl;
        }

        private static GameSpec GetTestSpecs()
        {
            return TestSpecs.Generate();
        }

        public static BuildingSpec RandomBuildingSpec()
        {
            return GameLogic.Specs.Buildings.Values.RandomElement();
        }


        public TileEntity RandomNotBuiltTile()
        {
            var tiles = World.AllTiles();
            foreach (var tile in tiles)
                if (tile.Components.Get<TileHabitants>().Building == null)
                    return tile;
            throw new System.Exception("No unbuilt tile");
        }
    }

}
