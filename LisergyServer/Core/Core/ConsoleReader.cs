using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LisergyServer.Core
{
    public class ConsoleReader
    {
        private Stream _consoleStream;
        private StreamReader _reader;
        private Task<string> _readTask;

        public ConsoleReader()
        {
            _consoleStream = Console.OpenStandardInput();
            _reader = new StreamReader(_consoleStream);
            _readTask = _reader.ReadLineAsync();
        }

        public bool TryReadConsoleText(out string cmd)
        {
            if(_readTask.IsCompleted)
            {
                cmd = _readTask.Result;
                _readTask = _reader.ReadLineAsync();
                return true;
            } else
            {
                cmd = null;
                return false;
            }
        }
    }
}
