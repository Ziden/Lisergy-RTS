using Assets.Code.Views;
using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.World
{
    public class Effects
    {
        private static Dictionary<Tile, GameObject> _effects = new Dictionary<Tile, GameObject>();

        public static void StopEffect(Tile t)
        {
            if (_effects.ContainsKey(t))
                MainBehaviour.Destroy(_effects[t]);
        }

        public static void BattleEffect(Tile t)
        {
            var view = GameView.Controller.GetView<TileView>(t);
            StopEffect(t);
            var prefab = Resources.Load("prefabs/BattleEffect");
            var obj = MainBehaviour.Instantiate(prefab, view.GameObject.transform) as GameObject;
            _effects[t] = obj;
            obj.transform.localPosition = new Vector3(0, 0.2f, 0);
        }
    }
}
