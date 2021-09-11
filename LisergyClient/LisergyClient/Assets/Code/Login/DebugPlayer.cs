using Assets.Code;
using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// To use when developing to make testing faster
public class DebugPlayer
{
    public static void CreateDebugPlayer()
    {
        MainBehaviour.Player = new ClientPlayer();
        var unit = new Unit();
        unit.Sprite = "knight";
        unit.Name = "Yolomaster";
        MainBehaviour.Player.AddUnit(unit);
    }
}

