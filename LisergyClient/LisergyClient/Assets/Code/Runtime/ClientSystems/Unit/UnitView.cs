using Assets.Code.Assets.Code.Assets;
using ClientSDK;
using Cysharp.Threading.Tasks;
using Game.Systems.Battler;
using System;
using UnityEngine;

namespace Assets.Code.World
{
    public class UnitView : IGameObject
    {
        public GameObject GameObject { get; set; }
        public Unit Unit;
        public UnitBehaviour Animations;
        private IAssetService _assets;
        private IGameClient _client;

        public UnitView(IGameClient client, in Unit unit)
        {
            _client = client;
            _assets = UnityServicesContainer.Resolve<IAssetService>();
            Unit = unit;
        }

        public async UniTask<GameObject> AddToScene()
        {
            var spec = _client.Game.Specs.Units[Unit.SpecId];
            GameObject = await _assets.CreatePrefab(spec.Art);
            GameObject.name = $"Unit Spec {Unit.SpecId}";
            Animations = GameObject.GetComponent<UnitBehaviour>();
            if (Animations == null)
            {
                Animations = GameObject.AddComponent<UnitBehaviour>();
            }
            return GameObject;
        }
    }
}
