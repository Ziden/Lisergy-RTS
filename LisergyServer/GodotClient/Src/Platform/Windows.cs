using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LisergyGodotClient.Src.Platform
{
    public interface IGamePlatform
    {
        void Initialize();
    }

#if GODOT_WINDOWS
    public class Windows : IGamePlatform
    {

        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();

        public void Initialize()
        {
            AllocConsole();
        }
    }
#endif
}
