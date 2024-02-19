using Game.Events.ServerEvents;
using ClientSDK.Data;
using Game.Tile;
using ClientSDK.SDKEvents;
using System.Collections.Generic;
using GameData;

namespace ClientSDK.Services
{
    /// <summary>
    /// Service responsible for handling authentication and specific account and profile information
    /// </summary>
    public interface IBuildingModule : IClientModule
    {
        IReadOnlyList<BuildingSpec> GetBuildingsKnown();
    }

    public class BuildingModule : IBuildingModule
    {
        private GameClient _client;
        private List<BuildingSpec> _known = new List<BuildingSpec>();

        public BuildingModule(GameClient client)
        {
            _client = client;
        }

        public void Register()
        {
            
        }

        public IReadOnlyList<BuildingSpec> GetBuildingsKnown()
        {
            _known.Clear();
            foreach (var b in _client.Game.Specs.Buildings.Values)
            {
                _known.Add(b);
            }
            return _known;
        }

    }
}
