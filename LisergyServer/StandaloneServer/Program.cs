using BaseServer;

public static class StandaloneProgram
{
    public static void Main(string[] args)
    {
        var standaloneServer = new StandaloneServer();
        standaloneServer.Multithreaded = true;
        standaloneServer.Start();
        standaloneServer.BlockThread();
    }
}