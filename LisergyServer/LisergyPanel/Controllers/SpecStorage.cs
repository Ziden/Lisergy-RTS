using GameData;
using GameData.Specs;
using GameDataTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LisergyPanel.Controllers
{
    public class SpecStorage
    {
        public static GameSpec GameSpecs;

        static SpecStorage()
        {
            if(GameSpecs == null)
            {
                GameSpecs = TestSpecs.Generate();
            }
        }
    }
}
