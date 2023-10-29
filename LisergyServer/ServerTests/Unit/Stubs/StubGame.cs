using Game;
using Game.DataTypes;
using Game.Network;
using Game.Scheduler;
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
        protected GameId _testPlayerId = GameId.Generate();
        public GameServerNetwork TestNetwork { get; protected set; }
        public BattleService BattleService { get; protected set; }
        public WorldService WorldService { get; protected set; }
        public CourseService CourseService { get; protected set; }
        public List<BasePacket> SentServerPackets { get; protected set; } = new List<BasePacket>();
        public GameWorld TestWorld { get; protected set; }
        public ServerChunkMap TestMap { get; protected set; }

        private static GameSpec _testSpecs;

        protected virtual GameWorld CreateTestWorld()
        {
            GameId.DEBUG_MODE = 1;
            Log.Debug("Setting Seed");
            WorldUtils.SetRandomSeed(666);
            Log.Debug("Creating World");
            TestWorld = new GameWorld(this, 16, 16);
            SetupWorld(TestWorld);
            Log.Debug("Clearing deltas");
            long originalByteCount = GC.GetTotalMemory(false) / 1000;
            Log.Debug($"Test World Ready: Heap Allocation Total {originalByteCount}kb");
            TestMap = TestWorld.Map as ServerChunkMap;
            return TestWorld;
        }

        public GameScheduler GameScheduler => this.Scheduler as GameScheduler;

        private static IGameLog GetTestLog()
        {
            var log = new GameLog("[Game]");
            log._Debug = m =>
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                if (lastLog != 0) m = $"[{now - lastLog}ms] " + m;
                lastLog = now;
                Console.WriteLine(m);
            };
            return log;
        }

        public TestGame(GameSpec specs = null, GameWorld world = null, bool createPlayer = true) : base(specs ?? GetTestSpecs(), GetTestLog())
        {
            CreateTestWorld();
            Log.Debug("Setting up serializer");
            Serialization.LoadSerializers();
            Log.Debug("Creating local services");
            BattleService = new BattleService(this);
            WorldService = new WorldService(this);
            CourseService = new CourseService(this);
            TestMap.SetFlag(0, 0, ChunkFlag.NEWBIE_CHUNK);
            TestNetwork = Network as GameServerNetwork;
            TestNetwork.OnOutgoingPacket += (player, packet) => ((TestServerPlayer)Players[player]).SendTestPacket(packet);
            if (createPlayer)
                CreatePlayer();
        }

        public void HandleClientEvent<T>(PlayerEntity sender, T ev) where T : BasePacket
        {
            //var deserialized = Serialization.ToCastedPacket<BasePacket>(Serialization.FromBasePacket(ev));
            //deserialized.Sender = sender;
            ev.Sender = sender;
            TestNetwork.IncomingPackets.Call(ev);
            Entities.DeltaCompression.SendDeltaPackets(sender);
        }

        public TestServerPlayer CreatePlayer(in int x = 10, in int y = 10)
        {
            Log.Debug("Creating new Test Player");
            var player = new TestServerPlayer(this);
            player.OnReceivedPacket += ev => ReceivePacket(ev);
            _testPlayerId = player.EntityId;
            var tile = World.Map.GetTile(x, y);
            player.EntityLogic.Player.PlaceNewPlayer(World.Map.GetTile(x, y));
            Entities.DeltaCompression.SendDeltaPackets(player);
            return player;
        }

        public void ReceivePacket(BasePacket ev)
        {
            SentServerPackets.Add(ev);
        }

        public TestServerPlayer GetTestPlayer()
        {
            PlayerEntity pl;
            this.World.Players.GetPlayer(_testPlayerId, out pl);
            return (TestServerPlayer)pl;
        }

        public static void BuildTestSpecs()
        {
            _testSpecs = TestSpecs.Generate();
            // Allow having initial building in tests
            _testSpecs.InitialBuildingSpecId = _testSpecs.Buildings[1].SpecId;
        }

        private static GameSpec GetTestSpecs()
        {
            if(_testSpecs == null)
            {
                BuildTestSpecs();
            }
            return _testSpecs;
        }

        public BuildingSpec RandomBuildingSpec()
        {
            return this.Specs.Buildings.Values.RandomElement();
        }


        public TileEntity RandomNotBuiltTile()
        {
            var tiles = TestWorld.AllTiles();
            foreach (var tile in tiles)
                if (tile.Building == null)
                    return tile;
            throw new System.Exception("No unbuilt tile");
        }

        private static long lastLog = 0;
    }

}
