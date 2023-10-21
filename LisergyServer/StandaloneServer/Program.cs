using Game;
using GameDataTest;
using MapServer;
using Terminal.Gui;
using GameDataTest.TestWorldGenerator;
using BaseServer.Core;
using NServiceBus.Logging;
using Telepathy;
using System.Runtime.CompilerServices;

var RUN_UI = true;


if(RUN_UI)
{
    var uiThread = Task.Run(() =>
    {
        Application.Run<StandaloneServerConsoleUI>();
        Application.Shutdown();
    });
    while (!StandaloneServerConsoleUI.IsLoaded) await Task.Yield();
}



var gameSpecs = TestSpecs.Generate();
var serverGameLog = StandaloneServerConsoleUI.HookLogs(new GameLog("[Game]"));
var game = new LisergyGame(gameSpecs, serverGameLog);
game.SetupGame(new TestWorld(), new GameServerNetwork(game));
game.Entities.DeltaCompression.ClearDeltas();

var server = new WorldServer(game, 1337);
StandaloneServerConsoleUI.HookLogs(server.Log);

game.Events.OnEventFired += StandaloneServerConsoleUI.OnReceiveEvent;
server.RunServer();