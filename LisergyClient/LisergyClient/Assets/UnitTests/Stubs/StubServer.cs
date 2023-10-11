using Assets.UnitTests.Stubs;
using Game;
using Game.DataTypes;
using Game.Events.ServerEvents;
using Game.Generator;
using Game.Network;
using Game.Network.ClientPackets;
using Game.Scheduler;
using Game.Services;
using Game.World;
using GameDataTest;
using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Assets.UnitTests
{
    public class StubServer
    {
        public static ConcurrentQueue<StubPacket> OutputStream = new ConcurrentQueue<StubPacket>();
        public static ConcurrentQueue<StubPacket> InputStream = new ConcurrentQueue<StubPacket>();

        WorldService _worldService;
        BattleService _battleService;
        CourseService _courseService;
        LisergyGame _game;

        public StubServer()
        {
            _game = SetupGame();
            _worldService = new WorldService(_game);
            _battleService = new BattleService(_game);
            _courseService = new CourseService(_game);
        }

        public static void OnClientSendToServer(byte[] bytes)
        {
            Log.Debug($"Sending bytes from client to server");
            InputStream.Enqueue(new StubPacket()
            {
                Data = bytes,
                Sender = MainBehaviour.LocalPlayer
            });
        }

        public void Tick()
        {
            /*
            GameScheduler.Tick(DateTime.UtcNow);
            while(InputStream.TryDequeue(out var packet)) {

                var deserialize = Serialization.ToEventRaw(packet.Data);
                if(deserialize is AuthPacket auth)
                {
                    auth.Sender = packet.Sender;
                    StubAuth(auth);
                } else
                {
                    Debug.Log("Server received input");
                    _game.ReceiveInput(packet.Sender, packet.Data);
                }
            }
            */
        }

        private LisergyGame SetupGame()
        {
            var gameSpecs = TestSpecs.Generate();
            _game = new LisergyGame(gameSpecs);

            (int, int) mapSize = TestWorldGenerator.MeasureWorld(20);

           // _game.SetWorld(new GameWorld(20, mapSize.Item1, mapSize.Item2);

            /*
            TestWorldGenerator.PopulateWorld(_game.World, 1234,
                new NewbieChunkPopulator(),
                new DungeonsPopulator()
            );
           
             */
           // DeltaTracker.Clear();
            return _game;
        }

        public void StubAuth(AuthPacket ev)
        {
            Debug.Log("Server received auth, returning to client");
            OutputStream.Enqueue(new StubPacket()
            {
                Data = Serialization.FromPacketRaw(new GameSpecPacket(_game)),
            });

            OutputStream.Enqueue(new StubPacket()
            {
                Data = Serialization.FromPacketRaw(new AuthResultPacket()
                {
                    PlayerID = GameId.Generate(),
                    Success = true
                }),
            });
        }
    }
}
