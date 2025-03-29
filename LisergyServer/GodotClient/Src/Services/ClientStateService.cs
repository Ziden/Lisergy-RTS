using ClientSDK;
using ClientSDK.SDKEvents;
using Game.Engine.ECLS;
using Game.Engine.Events.Bus;
using Game.Entities;
using Game.Tile;
using Godot;
using LisergyGodotClient.Src.Data;
using LisergyGodotClient.Src.Services.LisergyGodotClient.Src.Controllers;
using System;


namespace LisergyGodotClient.Src.Services
{
    public interface IClientStateService
    {
        ObservableProperty<IEntity> SelectedParty { get; }
        ObservableProperty<TileModel> SelectedTile { get; }
        ObservableProperty<Vector3> CameraPosition { get; }
        void ReceiveTapInput(Vector2 pos);
    }
    public class ClientStateService : IClientStateService, IEventListener
    {
        private IClientSDK _sdk;

        public ObservableProperty<IEntity> SelectedParty { get; } = new();
        public ObservableProperty<TileModel> SelectedTile { get; } = new();
        public ObservableProperty<Vector3> CameraPosition { get; } = new();

        public ClientStateService(IClientSDK sdk)
        {
            _sdk = sdk;
            _sdk.ClientEvents.On<EntitySeenEvent>(this, OnSeeEntity);
            _sdk.ClientEvents.On<GameStartedEvent>(this, OnGameStart);
        }

        private void OnGameStart(GameStartedEvent ev)
        {
            if (SelectedParty.Value == null)
            {
                var parties = ev.LocalPlayer.EntityLogic.GetParties();
                if (parties.Count == 0) return;
                SelectedParty.Value = parties[0];
            }
        }

        private void OnSeeEntity(EntitySeenEvent ev)
        {
            var e = ev.Entity;
            if(e.IsMine() 
                && e.EntityType == EntityType.Party 
                && SelectedParty.Value == null)
            {
                SelectedParty.Value = e;
            }
        }

        public void ReceiveTapInput(Vector2 pos)
        {
            var tile = _sdk.Game.World.GetTile(pos.ToLocation());
            if (tile == null) return;
            SelectedTile.Value = tile;
        }
    }
}
