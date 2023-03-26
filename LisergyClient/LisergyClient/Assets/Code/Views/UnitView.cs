using Assets.Code.Views;
using Game;
using Game.Battler;
using Game.ECS;
using UnityEngine;

namespace Assets.Code.World
{
    public class UnitView : IGameObject
    {
        public GameObject GameObject { get; set; }
        public Unit Unit;
        public Sprite3D Sprites;

        public UnitView(Unit unit)
        {
            Unit = unit;
        }

        public GameObject AddToScene()
        {
            if (GameObject == null)
            {
                StackLog.Debug($"Rendering unit {this}");
                var prefabName = Unit.GetSpec().Art.Name;
                var prefab = Resources.Load("prefabs/units/"+ prefabName);
                GameObject = MainBehaviour.Instantiate(prefab) as GameObject;
                GameObject.name = $"Unit Spec {Unit.SpecId}";
            }
            return GameObject;
        }

    }
}
