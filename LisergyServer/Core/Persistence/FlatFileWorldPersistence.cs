using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Game;
using Game.DataTypes;
using Game.ECS;
using Game.Systems.Map;
using Game.Systems.Player;
using Game.World;
using GameData;

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
        
        public async Task<LisergyGame> Load(GameSpec gameSpec, string worldName)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var worldDir = $"{currentDirectory}/{worldName}/";

            var playersTask = File.ReadAllBytesAsync(worldDir + "players.data");
            var entitiesTask = File.ReadAllBytesAsync(worldDir + "entities.data");
            var mapTask = File.ReadAllBytesAsync(worldDir + "map.data");

            await Task.WhenAll(playersTask, entitiesTask, mapTask);

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
        }

        public unsafe byte [] SerializeEntities(LisergyGame game)
        {
            return Serialization.FromAnyTypes(
                ((GameEntities)game.Entities).AllEntities
                .Select(e => new SerializedEntity(e))
                .ToList());
        }

        public void DeserializeEntities(LisergyGame game, byte[] entities)
        {
            var deserialized = Serialization.ToAnyTypes<SerializedEntity>(entities);
            foreach(var e in deserialized)
            {
                GameId.NextGeneration = e.EntityId;
                var created = game.Entities.CreateEntity(e.OwnerId, e.EntityType);
                foreach(var component in e.Components)
                {
                    created.Components.Save(component);
                }
                RepositionEntity(created);
            }
            game.Entities.DeltaCompression.ClearDeltas();
        }

        public byte[] SerializePlayers(LisergyGame game)
        {
            return Serialization.FromAnyTypes(game.World.Players.AllPlayers().Select(p => new SerializedPlayer(p)).ToList());
        }

        public void DeserializePlayers(LisergyGame game, byte[] players)
        {
            var deserialized = Serialization.ToAnyTypes<SerializedPlayer>(players);
            foreach(var p in deserialized)
            {
                GameId.NextGeneration = p.PlayerId;
                var newPlayer = new PlayerEntity(new PlayerProfile(p.PlayerId), game);
                newPlayer.Components.AddReference(p.Data);
                newPlayer.VisibilityReferences.OnceExplored = new HashSet<Position>(p.SeenTiles);
                game.Players.Add(newPlayer);

                // We clear the owned entities because it will be filled when entities are deserialized
                foreach(var k in p.Data.OwnedEntities.Keys)
                {
                    p.Data.OwnedEntities[k].Clear();
                }
            }
        }

        private void RepositionEntity(IEntity entity)
        {
            if(entity.Components.Has<MapPlacementComponent>())
            {
                var placement = entity.Components.Get<MapPlacementComponent>();
                entity.EntityLogic.Map.SetPosition(entity.Game.World.Map.GetTile(placement.Position.X, placement.Position.Y));
            }
        }

        public unsafe void DeserializeMap(LisergyGame game, byte[] data)
        {
            var map = (ServerChunkMap)game.World.Map;
            var chunkSize = sizeof(ChunkData);
            var qtdChunks = data.Length / chunkSize;
            var qtdChunksInMap = map.ChunkMapDimensions.x * map.ChunkMapDimensions.y;
            if (qtdChunksInMap != qtdChunks)
            {
                throw new Exception($"Byte array contains data for {qtdChunks} chunks but map has allocated only {qtdChunksInMap} chunks");
            }
            var offset = 0;
            fixed (byte* buffer = data)
            {
                foreach (var chunk in map.AllChunks())
                {
                    *chunk.Data.Pointer = *(ChunkData*)(buffer + offset);
                    offset += chunkSize;
                }
            }
        }

        public unsafe byte[] SerializeMap(LisergyGame game)
        {
            var map = (ServerChunkMap)game.World.Map;
            var chunkSize = sizeof(ChunkData);
            var totalChunks = map.ChunkMapDimensions.x * map.ChunkMapDimensions.y;
            var worldData = new byte[totalChunks * chunkSize + 4];
            var offset = 0;
            fixed (byte* buffer = worldData)
            {
                foreach (var chunk in map.AllChunks())
                {
                    *(ChunkData*)(buffer + offset) = *chunk.Data.Pointer;
                    offset += chunkSize;
                }
            }
            return worldData;
        }
    }
}
