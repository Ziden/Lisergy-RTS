using Assets.Code.Assets.Code.Assets;
using Game;
using Game.Battler;
using System;
using UnityEngine;

namespace Assets.Code.World
{
    public class UnitView : IGameObject
    {
        public GameObject GameObject { get; set; }
        public Unit Unit;
        public UnitMonoBehaviour UnitMonoBehaviour;
        private IAssetService _assets;


        public UnitView(Unit unit)
        {
            Unit = unit;
            _assets = ServiceContainer.Resolve<IAssetService>();
        }

        public void AddToScene(Action<GameObject> onAdded)
        {
            var prefabName = Unit.GetSpec().Art.Address;
            _assets.CreatePrefab(Unit.GetSpec().Art, Vector3.zero, Quaternion.Euler(0, 0, 0), o =>
            {
                GameObject = o;
                GameObject.name = $"Unit Spec {Unit.SpecId}";
                UnitMonoBehaviour = GameObject.GetComponent<UnitMonoBehaviour>();
                if (UnitMonoBehaviour == null)
                {
                    UnitMonoBehaviour = GameObject.AddComponent<UnitMonoBehaviour>();
                }
                onAdded(o);
            });
        }
    }
}
