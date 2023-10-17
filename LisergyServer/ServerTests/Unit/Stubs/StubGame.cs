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
        private GameId _testPlayerId = GameId.Generate();
        public GameServerNetwork TestNetwork { get; private set; }
        public BattleService BattleService { get; private set; }
        public WorldService WorldService { get; private set; }
        public CourseService CourseService { get; private set; }
        public List<BasePacket> SentServerPackets { get; private set; } = new List<BasePacket>();
        public GameWorld TestWorld { get; private set; }
        public PreAllocatedChunkMap TestMap { get; private set; }

        private GameWorld CreateTestWorld()
        {
            GameId.DEBUG_MODE = 1;
            WorldUtils.SetRandomSeed(666);
            TestWorld = new GameWorld(2, 16, 16);
            SetupGame(TestWorld, new GameServerNetwork(this));
            Entities.DeltaCompression.ClearDeltas();
            TestMap = TestWorld.Map as PreAllocatedChunkMap;
            return TestWorld;
        }

        public GameScheduler GameScheduler => this.Scheduler as GameScheduler;

        public TestGame(GameWorld world = null, bool createPlayer = true) : base(GetTestSpecs())
        {
            UnmanagedMemory.FlagMemoryToBeReused();
            CreateTestWorld();
            Serialization.LoadSerializers();
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
            var deserialized = Serialization.ToPacketRaw<BasePacket>(Serialization.FromPacketRaw(ev));
            deserialized.Sender = sender;
            TestNetwork.IncomingPackets.Call(deserialized);
            Entities.DeltaCompression.SendDeltaPackets(sender);
        }

        public TestServerPlayer CreatePlayer(in int x = 10, in int y = 10)
        {
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

        private static GameSpec GetTestSpecs()
        {
            return TestSpecs.Generate();
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

        static TestGame()
        {
            Log._Debug = m =>
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                if (lastLog != 0) m = $"[{now - lastLog}ms] " + m;
                lastLog = now;
                Console.WriteLine(m);
            };
        }
    }

}
