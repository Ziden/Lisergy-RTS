using Game;
using GameDataTest;
using MapServer;
using Terminal.Gui;
using GameDataTest.TestWorldGenerator;
using BaseServer.Core;

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

/*
var connectionHubThread = Task.Run(() =>
{
    var hub = new ConnectionHubServer(ConnectionHubNetwork.HUB_PORT);
    hub.RunServer();
});
*/

var gameSpecs = TestSpecs.Generate();
var game = new LisergyGame(gameSpecs);
game.SetupGame(new TestWorld(), new GameServerNetwork(game));

game.Entities.DeltaCompression.ClearDeltas();
var server = new StandaloneServer(game, ConnectionHubNetwork.HUB_PORT);
game.Events.OnEventFired += StandaloneServerConsoleUI.OnReceiveEvent;
server.RunServer();