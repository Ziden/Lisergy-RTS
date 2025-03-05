using BaseServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseServer.Commands
{

    public class ConsoleCommandExecutor
    {
        private static ConsoleSender _cmdSender;
        private static ConsoleReader _reader;
        public static readonly char CMD_CHAR = '.';

        private readonly Dictionary<string, Command> _commands = new Dictionary<string, Command>();
        private Command _cmd = null;
        private string _consoleText = null;
        private string[] _args = null;

        public List<Command> GetCommands()
        {
            return _commands.Values.ToList();
        }

        public ConsoleCommandExecutor(ConsoleReader reader)
        {
            _cmdSender = new ConsoleSender();
            _reader = reader;
        }

        public void HandleConsoleCommands()
        {
            if (_reader == null) return;
            if (_reader.TryReadConsoleText(out _consoleText))
            {
                if (_consoleText[0] != CMD_CHAR)
                {
                    _cmdSender.SendMessage("Command not found. Try .help");
                    return;
                }

                _args = _consoleText.Split(" ");
                if (_commands.TryGetValue(_args[0][1..], out _cmd))
                {
                    try
                    {
                        _cmd.Execute(_cmdSender, new CommandArgs(_args.Skip(1)));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
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
            _ = cmdString.Split(" ");
        }

        public void RegisterCommand(Command c)
        {
            _commands[c.GetCommand()] = c;
        }
    }
}
