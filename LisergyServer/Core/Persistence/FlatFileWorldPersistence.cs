using Game;
using Game.Engine;
using Game.Engine.ECLS;
using Game.Entities;
using Game.Systems.Map;
using GameData;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BaseServer.Persistence
{
    public enum SaveState
    {
        None, Writing, Reading
    }

    public interface IWorldPersistance
    {
        public SaveState State { get; }
    }

    /// <summary>
    /// For now just writes in flat files.
    /// Easy to change to a database, redis of whatever
    /// </summary>
    public class FlatFileWorldPersistence : IWorldPersistance
    {
        public SaveState State { get; private set; }

        private bool _world = false;
        private bool _entities = false;
        private IGameLog _log;

        public FlatFileWorldPersistence(IGameLog log)
        {
            _log = log;
        }

        public async Task Save(LisergyGame game, string worldName)
        {
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            var worldDir = $"{currentDirectory}/{worldName}/";
            _log.Info($"Writing World on Directory {worldDir}/");
            Directory.CreateDirectory(worldDir);

            var tasks = new List<Task>();
            tasks.Add(File.WriteAllBytesAsync(worldDir + "players.data", SerializePlayers(game)));
            tasks.Add(File.WriteAllBytesAsync(worldDir + "map.data", SerializeMap(game)));
            tasks.Add(File.WriteAllBytesAsync(worldDir + "entities.data", SerializeEntities(game)));
            await Task.WhenAll(tasks);
            _log.Info($"World Saved to {worldDir}");
        }

        public class MapData
        {
            public int SizeX;
            public int SizeY;
        }

        public async Task<LisergyGame> Load(GameSpec gameSpec, string worldName)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var worldDir = $"{currentDirectory}/{worldName}/";

            var playersTask = File.ReadAllBytesAsync(worldDir + "players.data");
            var entitiesTask = File.ReadAllBytesAsync(worldDir + "entities.data");
            var tilesTask = File.ReadAllBytesAsync(worldDir + "tiles.data");
            var mapTask = File.ReadAllBytesAsync(worldDir + "map.data");

            await Task.WhenAll(playersTask, entitiesTask, mapTask, tilesTask);
            return null;
            /*
            var qtdChunks = mapTask.Result.Length / Marshal.SizeOf<ChunkData>();
            var chunkSize = Math.Sqrt(qtdChunks);
            var tileSize = (ushort)(chunkSize * GameWorld.CHUNK_SIZE);
            var game = new LisergyGame(gameSpec, new GameLog("[Game]"));
            var world = new GameWorld(game, tileSize, tileSize);
            game.SetupWorld(world);

            DeserializeMap(game, mapTask.Result);
            DeserializePlayers(game, playersTask.Result);
            DeserializeEntities(game, entitiesTask.Result);
            return game;
            */
        }

        public unsafe byte[] SerializeEntities(LisergyGame game)
        {
            return Serialization.FromAnyTypes(
                ((GameEntities)game.Entities).AllEntities
                .Select(e => new SerializedEntity(e))
                .ToList());
        }

        public void DeserializeEntities(LisergyGame game, byte[] entities)
        {
            var deserialized = Serialization.ToAnyTypes<SerializedEntity>(entities);
            foreach (var e in deserialized)
            {
                var created = game.Entities.CreateEntity(e.EntityType, e.OwnerId, e.EntityId);
                foreach (var component in e.Components)
                {
                    created.Components.Save(component);
                }
                RepositionEntity(created);
            }
            game.Network.DeltaCompression.ClearDeltas();
        }

        public byte[] SerializePlayers(LisergyGame game)
        {
            return Serialization.FromAnyTypes(game.World.Players.AllPlayers().Select(p => new SerializedPlayer(p)).ToList());
        }

        public void DeserializePlayers(LisergyGame game, byte[] players)
        {
            /*
            var deserialized = Serialization.ToAnyTypes<SerializedPlayer>(players);
            foreach(var p in deserialized)
            {
                GameId.NextGeneration = p.PlayerId;

                var newPlayer = new PlayerWrapper(game);
                newPlayer.Save(new PlayerProfileComponent(p.PlayerId));
                newPlayer.Components.Save(p.Data);
                newPlayer.Components.Get<VisibilityData>().OnceExplored = new HashSet<Location>(p.SeenTiles);
                game.Players.Add(newPlayer);
            }
            */
        }

        private void RepositionEntity(IEntity entity)
        {
            if (entity.Components.Has<MapPlacementComponent>())
            {
                var placement = entity.Components.Get<MapPlacementComponent>();
                var tile = entity.Game.World.GetTile(placement.Position.X, placement.Position.Y);
                entity.Logic.Vision.UpdateVisionRange(null, tile); // because vision it's not serialized (but should)
            }
        }

        public unsafe void DeserializeMap(LisergyGame game, byte[] data)
        {
            /*
            var map = (ServerChunkMap)game.World.Map;
            foreach(var t in Serialization.ToAnyTypes<SerializedEntity>(data))
            {
                var pos = (TileDataComponent)t.Components.Where(c => c is TileDataComponent).First();
                var existing = map.GetTile(pos.Position);
                foreach(var c in t.Components)
                {
                    existing.Components.Save(c);
                }
            }
            */
        }

        public unsafe byte[] SerializeMap(LisergyGame game)
        {
            //var map = (ServerChunkMap)game.World.Map;
            //return Serialization.FromAnyTypes(map.AllTiles().Select(p => new SerializedEntity(p.TileEntity)).ToList());
            return null;
        }
    }
}
