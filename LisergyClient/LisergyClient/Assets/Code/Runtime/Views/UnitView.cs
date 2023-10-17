using Assets.Code.Assets.Code.Assets;
using ClientSDK;
using Game.Systems.Battler;
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
        private IGameClient _client;

        public UnitView(IGameClient client, in Unit unit)
        {
            _client = client;
            _assets = ClientServices.Resolve<IAssetService>();
            Unit = unit;
        }

        public void AddToScene(Action<GameObject> onAdded)
        {
            var spec = _client.Game.Specs.Units[Unit.SpecId];
            _assets.CreatePrefab(spec.Art, Vector3.zero, Quaternion.Euler(0, 0, 0), o =>
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
