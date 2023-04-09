using Assets.Code.Battle;
using Assets.Code.World;
using Game;
using Game.Entity;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using GameData;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Code
{
    public class ServerListener : IEventListener
    {
        private GameView _game;

        private WorldListener _worldService;
        private BattleListener _battleListener;
        private EntityListener _entityListener;

        public ServerListener(EventBus<ServerPacket> networkEvents)
        {
            networkEvents.Register<MessagePopupPacket>(this, Message);
            networkEvents.Register<GameSpecPacket>(this, ReceiveSpecs);
            _worldService = new WorldListener(networkEvents);
            _battleListener = new BattleListener(networkEvents);
            _entityListener = new EntityListener(networkEvents);
        }

        private void InitializeWorld(GameSpec spec, int sizeX, int sizeY)
        {
            _game = new GameView(spec, new ClientWorld(sizeX, sizeY));
        }

        [EventMethod]
        public void Message(MessagePopupPacket ev)
        {
            // TODO: Message factory
            Debug.LogWarning(ev.Args.First());
        }

        [EventMethod]
        public void ReceiveSpecs(GameSpecPacket ev)
        {
            Log.Debug("Received specs");
            if (_game != null)
                throw new Exception("Received to register specs twice");
            InitializeWorld(ev.Spec, ev.WorldX, ev.WorldY);
        }
    }
}
