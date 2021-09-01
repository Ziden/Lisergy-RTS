using Game;
using GameData.Specs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code
{
    public class LazyLoad
    {

        private static Dictionary<ushort, Sprite[]> _unitSprites = new Dictionary<ushort, Sprite[]>();

        private static Dictionary<string, Sprite[]> _sprites = new Dictionary<string, Sprite[]>();

        public static Sprite GetSprite(string name, int index)
        {
            if (_sprites.ContainsKey(name))
                return _sprites[name][index];

            var s = Resources.LoadAll<Sprite>("sprites/" + name);
            if(s.Length == 0) 
                throw new Exception("Sprite "+name+" not found");
            _sprites[name] = s;
            return s[index];
        }
    }
}
