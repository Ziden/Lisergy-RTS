using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Art
{
    public class ArtCache<Key, Type>
    {
        private Dictionary<Key, Type> _icons = new Dictionary<Key, Type>();

        private Func<Key, Type> _buildFunc;

        public ArtCache(Func<Key, Type> build)
        {
            _buildFunc = build;
        }

        public Type GetArt(Key key)
        {
            Type tex;
            if (!_icons.TryGetValue(key, out tex))
            {
                tex = _buildFunc(key);
                _icons[key] = tex;
            }
            return tex;
        }
    }
}
