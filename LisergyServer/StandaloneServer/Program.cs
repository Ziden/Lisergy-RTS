using Game;
using GameDataTest;
using MapServer;
using Terminal.Gui;
using GameDataTest.TestWorldGenerator;
using BaseServer.Core;
using NServiceBus.Logging;
using Telepathy;
using System.Runtime.CompilerServices;
using ServerTests.Integration.Stubs;

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

var standaloneServer = new StandaloneServer();
var world = standaloneServer.GetInstance<WorldServer>(ServerType.WORLD);
var account = standaloneServer.GetInstance<AccountServer>(ServerType.ACCOUNT);
var chat = standaloneServer.GetInstance<ChatServer>(ServerType.CHAT);

StandaloneServerConsoleUI.HookLogs(world.Log);
StandaloneServerConsoleUI.HookLogs(account.Log);
StandaloneServerConsoleUI.HookLogs(chat.Log);
world.Game.Events.OnEventFired += StandaloneServerConsoleUI.OnReceiveEvent;

standaloneServer.Start();
standaloneServer.BlockThread();