using ClientSDK;
using Game;
using Game.Network;
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

        private List<BasePacket> _received = new List<BasePacket>();

        public TestGameClient() : base()
        {
            _network = ClientNetwork as ClientNetwork;
            _network.OnReceiveGenericPacket += OnReceivePacket;
        }

        private void OnReceivePacket(BasePacket packet)
        {
            Log.Debug("Received Packet " + packet.GetType());
            _received.Add(packet);
        }

        public T GetLatestPacket<T>() where T : BasePacket 
        {
            return (T)_received.First(p => p.GetType() == typeof(T));
        }

        public async Task<T> WaitFor<T>() where T : BasePacket
        {
            var timeout = 10;
            _network.Tick();
            var p = _received.FirstOrDefault(p => p.GetType() == typeof(T));
            while (p == null && timeout >= 0)
            {
                timeout--;
                await Task.Delay(100);
                _network.Tick();
                p = _received.FirstOrDefault(p => p.GetType() == typeof(T));
            }
            
            return (T)p;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
