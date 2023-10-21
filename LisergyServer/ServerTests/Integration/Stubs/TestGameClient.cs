using ClientSDK;
using ClientSDK.Data;
using ClientSDK.SDKEvents;
using Game;
using Game.DataTypes;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using Game.Network;
using Game.Systems.Building;
using Game.Systems.Dungeon;
using Game.Systems.Party;
using Game.Tile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerTests.Integration.Stubs
{
    internal class TestGameClient : GameClient , IDisposable, IEventListener
    {
        public new ClientNetwork Network { get; private set; }
        public List<BasePacket> ReceivedPackets { get; private set; } = new List<BasePacket>();
        public List<IBaseEvent> EventsInClientLogic { get; private set; } = new List<IBaseEvent>();
        public TestGameClient() : base()
        {
            Network = base.Network as ClientNetwork;
            Network.OnReceiveGenericPacket += OnReceivePacket;
            this.ClientEvents.Register<GameStartedEvent>(this, OnGameStart);
        }

        /// <summary>
        /// When game starts we hook to game events for testing purposes
        /// </summary>
        private void OnGameStart(GameStartedEvent ev)
        {
            ev.Game.Events.OnEventFired += ev => EventsInClientLogic.Add(ev);
        }

        public void PrepareSDK()
        {
            GameId.DEBUG_MODE = 1;
            Modules.Views.RegisterView<TileEntity, EntityView<TileEntity>>();
            Modules.Views.RegisterView<PartyEntity, EntityView<PartyEntity>>();
            Modules.Views.RegisterView<DungeonEntity, EntityView<DungeonEntity>>();
            Modules.Views.RegisterView<PlayerBuildingEntity, EntityView<PlayerBuildingEntity>>();
        }

        private void OnReceivePacket(BasePacket packet)
        {
            if(!(packet is TilePacket)) Game?.Log?.Debug($"Received {packet} from server ");
            ReceivedPackets.Add(packet);
        }

        public T GetLatestPacket<T>() where T : BasePacket 
        {
            return (T)ReceivedPackets.First(p => p.GetType() == typeof(T));
        }

        public async Task<T> WaitFor<T>(Func<T, bool> validate = null) where T : BasePacket
        {
            var timeout = 10;
            Network.Tick();
            var p = ReceivedPackets.FirstOrDefault(p => p.GetType() == typeof(T));
            while (p == null && timeout >= 0)
            {
                timeout--;
                await Task.Delay(100);
                Network.Tick();
                p = ReceivedPackets.FirstOrDefault(p => p.GetType() == typeof(T) && (validate==null || validate((T)p)));
            }
            Network.Tick();
            return (T)p;
        }

        public void Dispose()
        {
            UnmanagedMemory.FreeAll();
        }
    }
}
