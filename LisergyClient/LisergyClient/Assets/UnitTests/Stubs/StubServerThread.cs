
using System.Threading;
using UnityEngine;

namespace Assets.UnitTests.Stubs
{
    public class StubServerThread
    {
        public static Thread ServerThread;
        public static bool Run = true;

        public static void SetupTestServer()
        {
            Object.Instantiate(new GameObject("TestRunnerBhv", typeof(TestBehaviour)));

            Networking.SenderOverride = StubServer.OnClientSendToServer;

            ServerThread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                var server = new StubServer();
                while(Run)
                {
                    server.Tick();
                    Thread.Sleep(100);
                }
            });
            ServerThread.Start();
        }
    }
}
