using Game;

namespace LisergyServer.Commands
{
    public class ConsoleSender : CommandSender
    {
        public override void SendMessage(string message)
        {
            Log.Info(message);
        }
    }
}
