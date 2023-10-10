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
using System.Collections.Generic;

namespace ServerTests
{
    public class TestGame : LisergyGame
    {
        private GameId _testPlayerId = GameId.Generate();
        public GameNetwork TestNetwork { get; private set; }
        public BattleService BattleService { get; private set; }
        public WorldService WorldService { get; private set; }
        public CourseService CourseService { get; private set; }
        public List<BasePacket> SentPackets { get; private set; } = new List<BasePacket>();


        private static GameWorld TestWorld;

        private GameWorld CreateTestWorld()
        {
            GameId.DEBUG_MODE = 1;
            WorldUtils.SetRandomSeed(666);
            UnmanagedMemory.FlagMemoryToBeReused();
            TestWorld = new GameWorld(4, 20, 20);
            SetWorld(TestWorld);
            Entities.DeltaCompression.ClearDeltas();
            TestWorld.AllocateMemory();
            return TestWorld;
        }

        public GameScheduler GameScheduler => this.Scheduler as GameScheduler;

        public TestGame(GameWorld world = null, bool createPlayer = true) : base(GetTestSpecs())
        {
            CreateTestWorld();
            Serialization.LoadSerializers();
            BattleService = new BattleService(this);
            WorldService = new WorldService(this);
            CourseService = new CourseService(this);
            this.World.Map.SetFlag(0, 0, ChunkFlag.NEWBIE_CHUNK);
            TestNetwork = Network as GameNetwork;
            TestNetwork.OnOutgoingPacket += (player, packet) => ((TestServerPlayer)Players[player]).SendTestPacket(packet);
            if (createPlayer)
                CreatePlayer();
        }

        public void HandleClientEvent<T>(PlayerEntity sender, T ev) where T : InputPacket
        {
            var deserialized = Serialization.ToPacketRaw<InputPacket>(Serialization.FromPacketRaw(ev));
            deserialized.Sender = sender;
            TestNetwork.IncomingPackets.Call(deserialized);
            Entities.DeltaCompression.SendDeltaPackets(sender);
        }

        public TestServerPlayer CreatePlayer(in int x = 10, in int y = 10)
        {
            var player = new TestServerPlayer(this);
            player.OnReceivedPacket += ev => ReceivePacket(ev);
            _testPlayerId = player.EntityId;
            var tile = this.World.GetTile(x, y);
            player.EntityLogic.Player.PlaceNewPlayer(this.World.GetTile(x, y));
            Entities.DeltaCompression.SendDeltaPackets(player);
            return player;
        }

        public void ReceivePacket(BasePacket ev)
        {
            SentPackets.Add(ev);
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
            var tiles = World.AllTiles();
            foreach (var tile in tiles)
                if (tile.Building == null)
                    return tile;
            throw new System.Exception("No unbuilt tile");
        }
    }

}
