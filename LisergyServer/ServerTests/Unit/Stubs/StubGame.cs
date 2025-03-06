using Game;
using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.Network;
using Game.Engine.Scheduler;
using Game.Services;
using Game.Systems.Player;
using Game.Tile;
using Game.World;
using GameData;
using GameDataTest;
using System;
using System.Collections.Generic;

namespace ServerTests
{
    public class TestGame : LisergyGame
    {
        protected GameId _testPlayerId;
        public GameServerNetwork TestNetwork { get; protected set; }
        public BattleService BattleService { get; protected set; }
        public WorldService WorldService { get; protected set; }
        public List<BasePacket> SentServerPackets { get; protected set; } = new List<BasePacket>();
        public GameWorld TestWorld => World as GameWorld;
        public ServerChunkMap WorldChunks => TestWorld.Chunks;

        private static GameSpec _testSpecs;

        protected virtual GameWorld CreateTestWorld()
        {
            Log.Debug("Setting Seed");
            WorldUtils.SetRandomSeed(666);
            Log.Debug("Creating World");
            var world = new GameWorld(this, 16, 16);
            SetupWorld(world);
            WorldChunks.SetOccupied(0, 0, false);
            return world;
        }

        public static void ValidateNoLeak(IGame game)
        {
            foreach (var e in game.Entities.AllEntities)
            {
                e.Components.ValidateComponentSetModifications();
            }
        }

        public TileModel FindTile(Func<TileModel, bool> filter)
        {
            foreach (var t in ((GameWorld)World).AllTiles())
            {
                if (filter(t))
                    return t;
            }
            return null;
        }

        public GameScheduler GameScheduler => this.Scheduler as GameScheduler;

        private static GameLog _testLog;

        private static IGameLog GetTestLog()
        {
            if (_testLog != null) return _testLog;

            _testLog = new GameLog("[Game]");
            _testLog._Debug = m =>
            {
                /*
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                if (lastLog != 0) m = $"[{now - lastLog}ms] " + m;
                lastLog = now;
                Console.WriteLine(m);
                */
            };
            return _testLog;
        }

        public TestGame(GameSpec specs = null, bool createWorld = true, bool createPlayer = true) : base(specs ?? GetTestSpecs(), GetTestLog())
        {
            GameId.INCREMENTAL_MODE = 1;
            if (createWorld)
            {
                CreateTestWorld();
            }
            Log.Debug("Setting up serializer");
            Serialization.LoadSerializers();
            Log.Debug("Creating local services");
            BattleService = new BattleService(this);
            WorldService = new WorldService(this);
            TestNetwork = Network as GameServerNetwork;
            GameScheduler.SetLogicalTime(DateTime.UtcNow);
            TestNetwork.OnOutgoingPacket += (player, packet) => (Players[player] as TestServerPlayer)?.SendTestPacket(packet);
            if (createPlayer && createWorld)
                CreatePlayer();
            long originalByteCount = GC.GetTotalMemory(false) / 1000;
            Log.Debug($"Test Game Ready: Heap Allocation Total {originalByteCount}kb");
        }

        public void HandleClientEvent<T>(PlayerModel sender, T ev) where T : BasePacket
        {
            //var deserialized = Serialization.ToCastedPacket<BasePacket>(Serialization.FromBasePacket(ev));
            var deserialized = ev;
            deserialized.Sender = sender;
            deserialized.SenderPlayerId = sender.EntityId;
            TestNetwork.IncomingPackets.Call(deserialized);
            if (deserialized is IGameCommand c)
            {
                c.Execute(sender.Game);
            }
            Network.DeltaCompression.SendAllModifiedEntities(sender.EntityId);
        }

        public TestServerPlayer CreatePlayer(in int x = 10, in int y = 10)
        {
            Log.Debug("Creating new Test Player");
            var player = new TestServerPlayer(this);
            player.OnReceivedPacket += ev => ReceivePacket(ev);
            _testPlayerId = player.EntityId;
            var tile = World.GetTile(x, y);
            Players.Add(player);
            player.EntityLogic.PlaceNewPlayer(World.GetTile(x, y));
            Network.DeltaCompression.SendAllModifiedEntities(player.EntityId);
            return player;
        }

        public void ReceivePacket(BasePacket ev)
        {
            SentServerPackets.Add(ev);
        }

        public TestServerPlayer GetTestPlayer()
        {
            PlayerModel pl;
            this.World.Players.GetPlayer(_testPlayerId, out pl);
            return (TestServerPlayer)pl;
        }


        public static void BuildTestSpecs()
        {
            if (_testSpecs == null)
            {
                _testSpecs = TestSpecs.Generate();
            }
            // Allow having initial building in tests
            _testSpecs.InitialBuildingSpecId = _testSpecs.Buildings[1].SpecId;
        }

        private static GameSpec GetTestSpecs()
        {
            if (_testSpecs == null)
            {
                Serialization.LoadSerializers();
                BuildTestSpecs();
            }
            return _testSpecs;
        }

        public BuildingSpec RandomBuildingSpec()
        {
            return this.Specs.Buildings.Values.RandomElement();
        }


        public TileModel RandomNotBuiltTile()
        {
            var tiles = TestWorld.AllTiles();
            foreach (var tile in tiles)
                if (tile.Logic.Tile.GetBuildingOnTile() == null)
                    return tile;
            throw new System.Exception("No unbuilt tile");
        }

        private static long lastLog = 0;
    }

}
