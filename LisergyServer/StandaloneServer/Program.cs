
using BaseServer;
using Game.Engine.ECLS;
using Game.World;
using MemoryPack;

[MemoryPackable]
public partial class Teste : IComponent
{
    Location Loc;
}

public static class StandaloneProgram
{



    public static void Main(string[] args)
    {
        MemoryPackSerializer.Serialize(new Teste());
        var standaloneServer = new StandaloneServer();
        standaloneServer.Multithreaded = true;
        standaloneServer.Start();
        standaloneServer.BlockThread();
    }
}