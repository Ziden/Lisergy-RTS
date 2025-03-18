using Game;
using System.Threading.Tasks;

namespace WebPlayerLogic
{
    public class CommandContext
    {
        public string PlayerId = string.Empty;
        public LisergyGame Game = null!;
    }

    public class SetupPlayerCommand
    {
        public Task Run(CommandContext context)
        {
            // Implementation will be added when player setup logic is defined
            return Task.CompletedTask;
        }
    }
}

