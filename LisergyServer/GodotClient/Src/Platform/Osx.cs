using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LisergyGodotClient.Src.Platform
{

#if OSX
    public class Osx : IGamePlatform
    {
        public void Initialize()
        {
        }
    }
#endif
}
