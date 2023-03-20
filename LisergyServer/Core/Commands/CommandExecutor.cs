using LisergyServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LisergyServer.Commands
{
    public class CommandExecutor
    {
        private static ConsoleSender _cmdSender;
        private static ConsoleReader _console;
        public static readonly char CMD_CHAR = '.';

        private Dictionary<string, Command> _commands = new Dictionary<string, Command>();
        private Command _cmd = null;
        private string _consoleText = null;
        private string[] _args = null;

        public List<Command> GetCommands()
        {
            return _commands.Values.ToList();
        }

        public CommandExecutor()
        {
            _cmdSender = new ConsoleSender();
            if (_console == null)
                _console = new ConsoleReader();
        }

        public void HandleConsoleCommands()
        {
            if (_console.TryReadConsoleText(out _consoleText))
            {
                if (_consoleText[0] != CMD_CHAR)
                {
                    _cmdSender.SendMessage("Command not found. Try .help");
                    return;
                }

                _args = _consoleText.Split(" ");
                if (_commands.TryGetValue(_args[0].Substring(1, _args[0].Length - 1), out _cmd))
                {
                    try
                    {
                        _cmd.Execute(_cmdSender, new CommandArgs(_args.Skip(1)));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    _cmdSender.SendMessage("Command not found. Try .help");
                }
            }
        }

        private void Invoke(Command cmd, string cmdString)
        {
            var args = cmdString.Split(" ");
        }

        public void RegisterCommand(Command c)
        {
            _commands[c.GetCommand()] = c;
        }
    }
}
