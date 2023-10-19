namespace WebGameLogic
{
    public interface IServerConfig
    {
        public string Title { get; }
        public string TitleKey { get; }
    }

    public class ServerConfig : IServerConfig
    {
        public string Title => "45CE6";
        public string TitleKey => "IW1M1H6FSUZKAYS3PGI1PZK88UB777SROC7S68B4478PC37GMG";
    }
}
