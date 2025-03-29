using Game.Engine;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisergyGodotClient.Src
{
    public class GodotLog : GameLog
    {
        public GodotLog(string tag) : base(tag)
        {
            SetupLog(this, ConsoleColor.Green);
        }

        public static void SetupLog(IGameLog ilog, ConsoleColor color)
        {
            ilog.InfoColor = color;
            ilog.DebugColor = color;
            GameLog log = (GameLog)ilog;
            log._Info = GD.Print;
            log._Debug = GD.Print;
            log._Error = GD.PrintErr;
        }

    }
}
