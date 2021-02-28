using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LisergyServer.Commands
{
    public class CommandArgs
    {
        private string [] _args;

        public CommandArgs(IEnumerable<string> args)
        {
            _args = args.ToArray();
        }

        public int Size
        {
            get => _args.Length;
        }

        public int GetInt(int index)
        {
            return int.Parse(_args[index]);
        }

        public double GetDouble(int index)
        {
            return double.Parse(_args[index]);
        }

        public bool GetBool(int index)
        {
            return bool.Parse(_args[index]);
        }

        public string GetString(int index)
        {
            return _args[index];
        }
    }
}
