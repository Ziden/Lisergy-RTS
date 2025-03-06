using ClientSDK;
using ClientSDK.Data;
using ClientSDK.SDKEvents;
using Game;
using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Engine.Events.Bus;
using Game.Engine.Network;
using Game.Entities;
using Game.Events.ServerEvents;
using Game.Systems.Map;
using Game.Systems.Player;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ServerTests.Integration.Stubs
{
    internal class TestGameClient : GameClient, IDisposable, IEventListener
    {
        private StandaloneServer _server;

        public new ClientNetwork Network { get; private set; }
        public List<BasePacket> ReceivedPackets { get; private set; } = new List<BasePacket>();
        public List<BasePacket> SentPackets { get; private set; } = new List<BasePacket>();
        public List<IBaseEvent> EventsInClientLogic { get; private set; } = new List<IBaseEvent>();
        public List<IClientEvent> EventsInSdk { get; private set; } = new List<IClientEvent>();
        public TestGameClient(StandaloneServer server) : base()
        {
            _server = server;
            Network = base.Network as ClientNetwork;
            Network.OnReceiveGenericPacket += OnReceivePacket;
            Network.OnSendGenericPacket += OnSentPacket;
            this.ClientEvents.On<GameStartedEvent>(this, OnGameStart);
        }

        public void SyncWithServer(LisergyGame serverGame, GameId _playerId)
        {
            serverGame.Network.ReceiveInput(_playerId, new JoinWorldMapCommand());
            var _serverPlayer = serverGame.Players[_playerId];
            Network.ReceiveInput(_playerId, new LoginResultPacket()
            {
                Profile = new PlayerProfileComponent(_playerId),
                Success = true
            });
            Network.ReceiveInput(_playerId, new GameSpecPacket(serverGame));
            SyncAllEntitiesOfType(_playerId, serverGame, EntityType.Tile);
            SyncAllEntitiesOfType(_playerId, serverGame, EntityType.Party);
            EventsInSdk.Clear();
            EventsInClientLogic.Clear();
            ReceivedPackets.Clear();
        }

        public void SyncAllEntitiesOfType(GameId viewerId, LisergyGame server, EntityType type)
        {
            foreach (var e in server.Entities.AllEntities.Where(e => e.EntityType == type))
            {
                Network.ReceiveInput(
                    viewerId, e.Logic.DeltaCompression.GetUpdatePacket(viewerId, false));
            }
        }

        /// <summary>
        /// When game starts we hook to game events for testing purposes
        /// </summary>
        private void OnGameStart(GameStartedEvent ev)
        {
            ClientEvents.OnEventFired += e => EventsInSdk.Add(e.FastShallowClone());
            ev.Game.Events.OnEventFired += e => EventsInClientLogic.Add(e.FastShallowClone());
        }

        public void ClearEvents()
        {
            ReceivedPackets.Clear();
            EventsInSdk.Clear();
            EventsInClientLogic.Clear();
            SentPackets.Clear();
        }

        public async Task PrepareSDK()
        {
            //GameId.DEBUG_MODE = 1;
            while (Network.ServersConnected.Count != 3)
            {
                Network.Tick();
                await Task.Delay(100);
            }
        }

        private void OnReceivePacket(BasePacket packet)
        {
            ReceivedPackets.Add(packet);
        }

        private void OnSentPacket(BasePacket packet)
        {
            SentPackets.Add(packet);
        }

        public List<T> FilterClientEvents<T>()
        {
            var all = new List<T>();
            var inLogic = EventsInClientLogic.Where(e => e.GetType() == typeof(T));
            var inSDK = EventsInSdk.Where(e => e.GetType() == typeof(T));
            foreach (var e in inLogic)
            {
                all.Add((T)e);
            }
            foreach (var e in inSDK)
            {
                all.Add((T)e);
            }
            return all;
        }

        public List<T> FilterReceivedPackets<T>()
        {
            return ReceivedPackets.Where(e => e.GetType() == typeof(T)).Cast<T>().ToList();
        }

        public async Task WaitUntilPacketCount<T>(int count, Func<T, bool> validate = null, int timeout = 20) where T : BasePacket
        {
            Network.Tick();
            _server?.SingleThreadTick();
            var p = ReceivedPackets.Count(p => p.GetType() == typeof(T) && (validate == null || validate((T)p)));
            while (p < count && timeout >= 0)
            {
                timeout--;
                await Task.Delay(100);
                Network.Tick();
                _server?.SingleThreadTick();
                p = ReceivedPackets.Count(p => p.GetType() == typeof(T) && (validate == null || validate((T)p)));
            }
            Network.Tick();
            _server?.SingleThreadTick();
            if (p < count) throw new Exception($"Client received {p} {typeof(T).Name} but expected {count}");
        }

        public async Task WaitUntilEventCount<T>(int count, int timeout = 20) where T : IBaseEvent
        {
            Network.Tick();
            _server?.SingleThreadTick();
            var p = FilterClientEvents<T>().Count();
            while (p < count && timeout >= 0)
            {
                timeout--;
                await Task.Delay(100);
                Network.Tick();
                _server?.SingleThreadTick();
                p = FilterClientEvents<T>().Count();
            }
            Network.Tick();
            _server?.SingleThreadTick();
            if (p < count) throw new Exception($"Client received {p} {typeof(T).Name} but expected {count}");
        }


        public T GetLatestPacket<T>() where T : BasePacket
        {
            return (T)ReceivedPackets.First(p => p.GetType() == typeof(T));
        }

        public async Task<T> WaitForEntityComponentUpdate<T>(IEntity e) where T : IComponent
        {
            Tick();
            var updates = FilterReceivedPackets<EntityUpdatePacket>().Where(p => p.EntityId == e.EntityId && p.SyncedComponents.Any(c => c.GetType() == typeof(T)));
            var tries = 10;
            while (tries > 0 && updates.Count() == 0)
            {
                tries--;
                Tick();
                updates = FilterReceivedPackets<EntityUpdatePacket>().Where(p => p.EntityId == e.EntityId && p.SyncedComponents.Any(c => c.GetType() == typeof(T)));
                await Task.Delay(100);
            }
            Assert.Greater(updates.Count(), 0, $"Did not receive component {typeof(T).Name} update from entity {e.EntityId}");
            return updates.Last().GetComponent<T>();
        }

        public async Task WaitUntilSends<T>(Func<T, bool> validate = null, int timeout = 20) where T : BasePacket
        {
            Network.Tick();
            _server?.SingleThreadTick();
            var p = SentPackets.FirstOrDefault(p => p.GetType() == typeof(T) && (validate == null || validate((T)p)));
            while (p == null && timeout >= 0)
            {
                timeout--;
                await Task.Delay(100);
                Network.Tick();
                _server?.SingleThreadTick();
                p = SentPackets.FirstOrDefault(p => p.GetType() == typeof(T) && (validate == null || validate((T)p)));
            }
            if (timeout == 0) p = null;
            Network.Tick();
            _server?.SingleThreadTick();
            if (p == null) throw new Exception("Client did not sent packet " + typeof(T).Name + " as expected");
        }

        public void Tick()
        {
            Network.Tick();
            _server?.SingleThreadTick();
        }

        public async Task<T> WaitUntilReceives<T>(Func<T, bool> validate = null, int timeout = 20) where T : BasePacket
        {
            Network.Tick();
            _server?.SingleThreadTick();
            var p = ReceivedPackets.FirstOrDefault(p => p.GetType() == typeof(T) && (validate == null || validate((T)p)));
            while (p == null && timeout >= 0)
            {
                timeout--;
                await Task.Delay(100);
                Network.Tick();
                _server?.SingleThreadTick();
                p = ReceivedPackets.FirstOrDefault(p => p.GetType() == typeof(T) && (validate == null || validate((T)p)));
            }
            if (timeout == 0) p = null;
            Network.Tick();
            return (T)p;
        }

        public void Dispose()
        {
            UnmanagedMemory.FreeAll();
        }
    }
}
