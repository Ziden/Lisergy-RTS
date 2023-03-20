using System;
using System.IO;
using System.Threading.Tasks;

namespace BaseServer.Core
{
    public class ConsoleReader
    {
        private readonly StreamReader _reader;
        private Task<string> _readTask;

        public ConsoleReader()
        {
            Stream consoleStream = Console.OpenStandardInput();
            _reader = new StreamReader(consoleStream);
            _readTask = _reader.ReadLineAsync();
        }

        public bool TryReadConsoleText(out string cmd)
        {
            if (_readTask.IsCompleted)
            {
                cmd = _readTask.Result;
                _readTask = _reader.ReadLineAsync();
                return true;
            }
            else
            {
                cmd = null;
                return false;
            }
        }
    }
}
