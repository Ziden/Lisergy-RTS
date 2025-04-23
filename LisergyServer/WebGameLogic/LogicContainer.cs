using Game;

namespace WebPlayerLogic;

public class CommandContext
{
	public LisergyGame Game = null!;
	public string PlayerId = string.Empty;
}

public class SetupPlayerCommand
{
	public Task Run(CommandContext context)
	{
		// Implementation will be added when player setup logic is defined
		return Task.CompletedTask;
	}
}