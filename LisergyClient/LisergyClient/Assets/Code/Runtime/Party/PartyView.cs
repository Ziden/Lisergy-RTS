using Assets.Code.Views;
using Game.ECS;
using System.Collections.Generic;
using Assets.Code.Code.Utils;
using UnityEngine;
using Game.Systems.Party;
using Game.Systems.Battler;
using Assets.Code.Assets.Code.Assets;

namespace Assets.Code.World
{
    public partial class PartyView : UnityEntityView<PartyEntity>
    {
        private static GameObject _container;

        private IAssetService _assets;

        private Dictionary<Unit, UnitView> _unitObjects = new Dictionary<Unit, UnitView>();

        public IReadOnlyDictionary<Unit, UnitView> UnitObjects => _unitObjects;

        /*
        protected override void InstantiationImplementation()
        {
            var o = new GameObject($"{Entity.OwnerID}-{Entity.EntityId}");
            o.transform.SetParent(Container.transform);
            o.transform.position = Entity.Tile.Position(0.2f);
            SetGameObject(o);
            o.SetActive(true);
            Log.Debug($"Created new party instance {this}");
        }
        */

        private static GameObject Container => _container = _container ?? new GameObject("Parties");

        /*
        public void CreateUnitObjects() 
        {
            foreach (var unit in Entity.Get<BattleGroupComponent>().Units)
            {
                var unitObject = new UnitView(unit);
                unitObject.AddToScene(o =>
                {
                    o.transform.SetParent(GameObject.transform);
                    o.transform.localPosition = Vector3.zero;
                    _unitObjects[unit] = unitObject;
                });
            }
        }    
        */
    }
}
