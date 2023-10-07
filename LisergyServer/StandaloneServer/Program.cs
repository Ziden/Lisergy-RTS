using Game.Generator;
using Game.Network;
using Game.World;
using Game;
using GameDataTest;
using MapServer;
using Terminal.Gui;
using GameDataTest.TestWorldGenerator;

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
var game = new LisergyGame(gameSpecs);
game.SetWorld(new TestWorld());
game.Entities.DeltaCompression.ClearDeltas();

var server = new StandaloneServer(game, 1337);
game.Events.OnEventFired += StandaloneServerConsoleUI.OnReceiveEvent;
server.RunServer();

