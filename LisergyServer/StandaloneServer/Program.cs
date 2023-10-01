using MapServer;
using Terminal.Gui;

var RUN_UI = true;

if(RUN_UI)
{
    var uiThread = Task.Run(() =>
    {
        Application.Run<StandaloneServer>();
        Application.Shutdown();
    });
    while (!StandaloneServer.IsLoaded) await Task.Yield();
}


var server = new MapSocketServer(1337);
server.RunServer();

