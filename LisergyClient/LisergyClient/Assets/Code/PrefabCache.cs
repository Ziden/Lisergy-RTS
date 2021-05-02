using GameData.Specs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code
{
    public class PrefabCache
    {
        private static Dictionary<string, UnityEngine.Object> _loaded = new Dictionary<string, UnityEngine.Object>();

        public static string GetPrefabPath(string folder)
        {
            return $"prefabs/{folder}/";
        }

        public static UnityEngine.Object GetArt(string folder, ArtSpec spec)
        {
            var path = GetPrefabPath(folder) + spec.Name;
            if (!_loaded.ContainsKey(path))
                _loaded[path] = Resources.Load(path);
            return _loaded[path];
        }
    }
}
