using ClientSDK;
using ClientSDK.Data;
using Game;
using Game.ECS;
using Game.Events.GameEvents;
using Game.Tile;
using GameAssets;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Assets.Code.Views
{
    public class TileService : IGameService
    {
        private IGameClient _client;



        public TileService(IGameClient c)
        {
            _client = c;
            _client.Game.Systems.TileVisibility.On<EntityTileVisibilityUpdateEvent>(OnVisibilityChange);
        }

        private void OnVisibilityChange(IEntity tileEntity, EntityTileVisibilityUpdateEvent ev)
        {
            var view = _client.Modules.Views.GetView<TileView>(tileEntity);
            if (view.State != EntityViewState.RENDERED)
                return;

            if (!ev.Explored)
            {
                view.SetFogInTileView(true, true);
            }
            else
            {
                view.SetFogInTileView(false, true);
                view.GameObject.SetActive(ev.Explored);
            }
        }

        public void OnSceneLoaded()
        {
            
        }
    }
}
