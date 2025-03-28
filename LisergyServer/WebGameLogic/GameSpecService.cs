// Services/GameSpecService.cs
using GameData;
using GameData.Specs;
using Newtonsoft.Json;
using System.Text;

namespace WebPlayerLogic.Services
{
    public class GameSpecService
    {
        private readonly string _gameSpecFilePath;
        private GameSpec? _cachedGameSpec;
        private readonly ILogger<GameSpecService> _logger;

        public GameSpecService(IWebHostEnvironment env, ILogger<GameSpecService> logger)
        {
            _logger = logger;
            _gameSpecFilePath = Path.Combine(env.ContentRootPath, "Data", "GameSpec.json");
            
            // Ensure data directory exists
            var directory = Path.GetDirectoryName(_gameSpecFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }
        }

        public GameSpec GetGameSpec()
        {
            if (_cachedGameSpec != null)
                return _cachedGameSpec;

            if (!File.Exists(_gameSpecFilePath))
            {
                _cachedGameSpec = new GameSpec(1);
                SaveGameSpec(_cachedGameSpec);
                return _cachedGameSpec;
            }

            var json = File.ReadAllText(_gameSpecFilePath);
            _cachedGameSpec = JsonConvert.DeserializeObject<GameSpec>(json);
            return _cachedGameSpec!;
        }

        public void SaveGameSpec(GameSpec gameSpec)
        {
            var json = JsonConvert.SerializeObject(gameSpec, Formatting.Indented);
            File.WriteAllText(_gameSpecFilePath, json);
            _cachedGameSpec = gameSpec;
        }

        // CRUD operations for each component
        public Dictionary<byte, BuildingSpec> GetBuildings() => GetGameSpec().Buildings;

        public BuildingSpec? GetBuilding(byte id) => GetGameSpec().Buildings.TryGetValue(id, out var building) ? building : null;

        public void AddBuilding(byte id, BuildingSpec building)
        {
            var gameSpec = GetGameSpec();
            gameSpec.Buildings[id] = building;
            SaveGameSpec(gameSpec);
        }

        public bool UpdateBuilding(byte id, BuildingSpec building)
        {
            var gameSpec = GetGameSpec();
            if (!gameSpec.Buildings.ContainsKey(id))
                return false;

            gameSpec.Buildings[id] = building;
            SaveGameSpec(gameSpec);
            return true;
        }

        public bool DeleteBuilding(byte id)
        {
            var gameSpec = GetGameSpec();
            if (!gameSpec.Buildings.ContainsKey(id))
                return false;

            gameSpec.Buildings.Remove(id);
            SaveGameSpec(gameSpec);
            return true;
        }

        // Similar methods for other components (Units, Resources, etc.)
    }
}
