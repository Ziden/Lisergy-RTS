using ClientSDK.Services;
using Game.Engine.ECLS;
using Game.Systems.Map;
using System.ComponentModel;

namespace ClientSDK.Modules
{
    /// <summary>
    /// Module responsible for handling base component logic to keep the client logic "up to date"
    /// This module single responsibility is to try to keep the client game in sync with server game as most as it can
    /// The only thing this won't keep track is the player data which is handled in <see cref="IPlayerModule"/>
    /// </summary>
    public class SystemSynchronizer
    {
        private GameClient _gameClient;

        public SystemSynchronizer(GameClient gameClient)
        {
            _gameClient = gameClient;
        }

        public void ListenForRequiredSyncs()
        {
            _gameClient.Modules.Entities.OnComponentUpdate<MapPlacementComponent>(OnUpdatePlacement);
            _gameClient.Modules.Entities.OnComponentAdded<MapPlacementComponent>(OnAddPlacement);
        }

        private void OnAddPlacement(IEntity entity, MapPlacementComponent component)
        {
            entity.Logic.Vision.UpdateVisionRange(null, _gameClient.Game.World.GetTile(component.Position));
        }

        /// <summary>
        /// Whenever map placement updates are sent to the client we re-position the entity on the client logic.
        /// This will trigger all exploration events
        /// </summary>
        private void OnUpdatePlacement(IEntity entity, MapPlacementComponent oldValue, MapPlacementComponent newValue)
        {
            if (oldValue.Position != newValue.Position)
            {
                var oldTile = _gameClient.Game.World.GetTile(oldValue.Position.X, oldValue.Position.Y);
                var newTile = _gameClient.Game.World.GetTile(newValue.Position.X, newValue.Position.Y);
                entity.Logic.Vision.UpdateVisionRange(oldTile, newTile);
            }
        }
    }
}
