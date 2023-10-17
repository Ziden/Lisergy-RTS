using Assets.Code.Views;
using Game.ECS;
using System.Collections.Generic;
using UnityEngine;
using Game.Systems.Party;
using Game.Systems.Battler;
using Game;
using ClientSDK.Data;

namespace Assets.Code.World
{
    public partial class PartyView : UnityEntityView<PartyEntity>
    {
        private Dictionary<Unit, UnitView> _unitObjects = new Dictionary<Unit, UnitView>();

        public IReadOnlyDictionary<Unit, UnitView> UnitObjects => _unitObjects;

        protected override void CreateView()
        {
            GameObject = new GameObject($"Party {Entity.EntityId} from {Entity.OwnerID}");
            GameObject.transform.SetParent(ViewContainer.transform);
            GameObject.transform.position = Entity.UnityPosition();
            UpdateUnits();
            State = EntityViewState.RENDERED;
            Log.Debug($"Created new party instance {this}");
        }

        /// <summary>
        /// Reads the battle group component of the party and draws the units
        /// </summary>
        public void UpdateUnits()
        {
            foreach (var unit in Entity.Get<BattleGroupComponent>().Units)
            {
                var unitObject = new UnitView(Client, unit);
                unitObject.AddToScene(o =>
                {
                    o.transform.SetParent(GameObject.transform);
                    o.transform.localPosition = Vector3.zero;
                    _unitObjects[unit] = unitObject;
                });
            }
        }
    }
}
