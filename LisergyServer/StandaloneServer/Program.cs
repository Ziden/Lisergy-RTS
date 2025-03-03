using Game.Engine;
using MapServer;
using ServerTests.Integration.Stubs;
using Terminal.Gui;

bool UI = false;

// Load Console UI on its own thread
if (UI)
{
    _ = Task.Run(() =>
    {
        Application.Run<StandaloneServerConsoleUI>();
        Application.Shutdown();
    });
    while (!StandaloneServerConsoleUI.IsLoaded) await Task.Yield();
}

// Load standalone which runs every server in its own thread
var standaloneServer = new StandaloneServer();
var world = standaloneServer.GetInstance<WorldServer>(ServerType.WORLD);
var account = standaloneServer.GetInstance<AccountServer>(ServerType.ACCOUNT);
var chat = standaloneServer.GetInstance<ChatServer>(ServerType.CHAT);

// hook all server logs to the console ui
if (UI)
{
    StandaloneServerConsoleUI.HookLogs(world.Log);
    StandaloneServerConsoleUI.HookLogs(account.Log);
    StandaloneServerConsoleUI.HookLogs(chat.Log);
    world.Game.Events.OnEventFired += StandaloneServerConsoleUI.OnReceiveEvent;
}

AppDomain.CurrentDomain.ProcessExit += (e, a) =>
{
    // standaloneServer.SaveWorld("TestWorld");
};
standaloneServer.Multithreaded = true;
standaloneServer.Start();
standaloneServer.BlockThread();