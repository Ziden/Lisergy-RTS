using ClientSDK;
using ClientSDK.Data;
using Game;
using Game.Network;
using Game.Systems.Dungeon;
using Game.Systems.Party;
using Game.Tile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTests.Integration.Stubs
{
    internal class TestGameClient : GameClient , IDisposable
    {
        private ClientNetwork _network;

        public List<BasePacket> ReceivedPackets { get; private set; } = new List<BasePacket>();

        public TestGameClient() : base()
        {
            _network = Network as ClientNetwork;
            _network.OnReceiveGenericPacket += OnReceivePacket;

            Modules.Views.RegisterView<TileEntity, EntityView<TileEntity>>();
            Modules.Views.RegisterView<PartyEntity, EntityView<PartyEntity>>();
            Modules.Views.RegisterView<DungeonEntity, EntityView<DungeonEntity>>();
        }

        private void OnReceivePacket(BasePacket packet)
        {
            Log.Debug("Received Packet from server " + packet.GetType());
            ReceivedPackets.Add(packet);
        }

        public T GetLatestPacket<T>() where T : BasePacket 
        {
            return (T)ReceivedPackets.First(p => p.GetType() == typeof(T));
        }

        public async Task<T> WaitFor<T>() where T : BasePacket
        {
            var timeout = 10;
            _network.Tick();
            var p = ReceivedPackets.FirstOrDefault(p => p.GetType() == typeof(T));
            while (p == null && timeout >= 0)
            {
                timeout--;
                await Task.Delay(100);
                _network.Tick();
                p = ReceivedPackets.FirstOrDefault(p => p.GetType() == typeof(T));
            }
            _network.Tick();
            return (T)p;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
