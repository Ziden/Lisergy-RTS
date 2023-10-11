using Assets.Code.Battle;
using Assets.Code.World;
using Game;
using Game.Entity;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using Game.Network;
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

        public ServerListener(EventBus<BasePacket> networkEvents)
        {
            networkEvents.Register<MessagePacket>(this, Message);
            networkEvents.Register<GameSpecPacket>(this, ReceiveSpecs);
            _worldService = new WorldListener(networkEvents);
            _battleListener = new BattleListener(networkEvents);
            _entityListener = new EntityListener(networkEvents);
        }

        private void InitializeWorld(GameSpec spec, ushort sizeX, ushort sizeY)
        {
            _game = new GameView(spec, new ClientWorld(sizeX, sizeY));
        }

        public void Message(MessagePacket ev)
        {
            // TODO: Message factory
            Debug.LogWarning(ev.Args.First());
        }

        public void ReceiveSpecs(GameSpecPacket ev)
        {
            Log.Debug("Received specs");
            if (_game != null)
                throw new Exception("Received to register specs twice");
            InitializeWorld(ev.Spec, (ushort)ev.WorldX, (ushort)ev.WorldY);
        }
    }
}
