using ClientSDK;
using ClientSDK.SDKEvents;
using Game.Engine.ECLS;
using Game.Engine.Events.Bus;
using Game.Entities;
using Game.Tile;
using Godot;
using LisergyGodotClient.Src.Services.LisergyGodotClient.Src.Controllers;
using System;


namespace LisergyGodotClient.Src.Services
{
    public interface IClientStateService
    {
        IEntity SelectedParty { get; }
        TileModel SelectedTile { get; }
        Vector3 CameraPosition { get; }

        event Action<TileModel> OnTileSelected;
        event Action<Vector3> OnCameraMoved;
        event Action<IEntity> OnPartySelected;

        void SetSelectedTile(TileModel e);
        void SetCameraPosition(Vector3 e);
        void SetSelectedParty(IEntity e);
        void ReceiveTapInput(Vector2 pos);
    }
    public class ClientStateService : IClientStateService, IEventListener
    {
        private IClientSDK _sdk;
        public event Action<TileModel> OnTileSelected;
        public event Action<Vector3> OnCameraMoved;
        public event Action<IEntity> OnPartySelected;

        public ClientStateService(IClientSDK sdk)
        {
            _sdk = sdk;
            _sdk.ClientEvents.On<EntitySeenEvent>(this, OnSeeEntity);
            _sdk.ClientEvents.On<GameStartedEvent>(this, OnGameStart);
        }

        private void OnGameStart(GameStartedEvent ev)
        {
            if (SelectedParty == null)
            {
                var parties = ev.LocalPlayer.EntityLogic.GetParties();
                if (parties.Count == 0) return;
                SetSelectedParty(parties[0]);
            }
        }

        private void OnSeeEntity(EntitySeenEvent ev)
        {
            var e = ev.Entity;
            if(e.IsMine() 
                && e.EntityType == EntityType.Party 
                && SelectedParty == null)
            {
                SetSelectedParty(e);
            }
        }

        public void ReceiveTapInput(Vector2 pos)
        {
            var tile = _sdk.Game.World.GetTile(pos.ToLocation());
            if (tile == null) return; // TODO: Check || !tile.Entity.IsVisible()
            SetSelectedTile(tile);
        }

        public TileModel SelectedTile { get; private set; }

        public Vector3 CameraPosition { get; private set; }
        public IEntity SelectedParty { get; private set; }

        public void SetSelectedTile(TileModel e)
        {
            SelectedTile = e;
            OnTileSelected?.Invoke(e);
        }

        public void SetCameraPosition(Vector3 e)
        {
            CameraPosition = e;
            OnCameraMoved?.Invoke(e);
        }

        public void SetSelectedParty(IEntity e)
        {
            SelectedParty = e;
            OnPartySelected?.Invoke(e);
        }
    }
}
