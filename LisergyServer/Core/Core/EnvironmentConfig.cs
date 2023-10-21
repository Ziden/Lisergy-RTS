using Game.DataTypes;

namespace BaseServer.Core
{
    public class EnvironmentConfig
    {
        /// <summary>
        /// Secret key user for inter server communication
        /// </summary>
        public static readonly GameId SECRET_KEY = new GameId(666, 6969);
    }
}
