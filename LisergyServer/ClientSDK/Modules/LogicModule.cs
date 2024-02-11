using ClientSDK.Services;
using Game.ECS;
using Game.Systems.Map;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSDK.Modules
{
    /// <summary>
    /// Module responsible for handling base component logic to keep the client logic "up to date"
    /// This module single responsibility is to try to keep the client game in sync with server game as most as it can
    /// The only thing this won't keep track is the player data which is handled in <see cref="IPlayerModule"/>
    /// </summary>
    public interface ILogicModule : IClientModule { }

    public class LogicModule : ILogicModule
    {
        private GameClient _gameClient;

        public LogicModule(GameClient gameClient)
        {
            _gameClient = gameClient;
        }

        public void Register()
        {
            _gameClient.Modules.Components.OnComponentUpdate<MapPlacementComponent>(OnUpdatePlacement);
        }

        /// <summary>
        /// Whenever map placement updates are sent to the client we re-position the entity on the client logic.
        /// This will trigger all exploration events
        /// </summary>
        private void OnUpdatePlacement(IEntity entity, MapPlacementComponent oldValue, MapPlacementComponent newValue) 
        {
            if(oldValue.Position != newValue.Position)
            {
                var newTile = _gameClient.Game.World.Map.GetTile(newValue.Position.X, newValue.Position.Y);
                entity.EntityLogic.Map.SetPosition(newTile);
            }
        }
    }
}
