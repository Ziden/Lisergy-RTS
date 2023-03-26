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

        public static Sprite GetSpecificSpriteArt(ArtSpec art)
        {
            if (_sprites.ContainsKey(art.Name))
                return _sprites[art.Name][art.Index];

            var s = Resources.LoadAll<Sprite>("sprites/" + art.Name);
            _sprites[art.Name] = s;
            return s[art.Index];
        }

        public static Sprite[] GetSprite(ushort unitSpecID)
        {
            Sprite[] response;
            if(!_unitSprites.TryGetValue(unitSpecID, out response))
            {
                var art = StrategyGame.Specs.Units[unitSpecID].Art;
                response = Resources.LoadAll<Sprite>("sprites/" + art.Name);
                if (response == null || response.Length <= 1)
                    throw new Exception("Error finding sprite " + art.Name+" for unitSpec "+ unitSpecID);
                _unitSprites[unitSpecID] = response;
               
            }
            return response;
        }
    }
}
