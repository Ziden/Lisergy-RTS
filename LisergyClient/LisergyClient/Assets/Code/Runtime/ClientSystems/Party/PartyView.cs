using UnityEngine;
using ClientSDK.Data;
using Assets.Code.Assets.Code.Runtime.Movement;
using ClientSDK;
using Game.Systems.BattleGroup;
using System.Collections.Generic;
using Game.Engine.ECLS;

namespace Assets.Code.World
{
    public partial class PartyView : UnityEntityView, IEntityMovementInterpolated
    {
        public PartyView(IGameClient client, IEntity e) : base(e, client) { }

        public MovementInterpolator MovementInterpolator { get; private set; }

        public IReadOnlyCollection<UnitView> UnitViews => Entity.Components.Get<BattleGroupUnitsComponent>().UnitViews.Values;

        protected override void CreateView()
        {
            GameObject = new GameObject($"Party {Entity.EntityId} from {Entity.OwnerID}");
            GameObject.transform.SetParent(ViewContainer.transform);
            MovementInterpolator = new MovementInterpolator(Client, Entity);
            State = EntityViewState.RENDERED;
            Client.Log.Debug($"Created new party instance {this}");
        }

        public void UpdateUnits(UnitGroup group)
        {
            var entity = this.Entity;
            if (!entity.Components.Has<BattleGroupUnitsComponent>())
            {
                entity.Components.Add<BattleGroupUnitsComponent>();
            }
            var unitsComponent = entity.Components.Get<BattleGroupUnitsComponent>();
            foreach (var unit in unitsComponent.UnitViews.Values) GameObject.Destroy(unit.GameObject);
            unitsComponent.UnitViews.Clear();
            foreach (var unit in group)
            {
                var unitObject = new UnitView(Client, unit);
                unitObject.AddToScene(o =>
                {
                    o.transform.SetParent(GameObject.transform);
                    o.transform.localPosition = Vector3.zero;
                    unitsComponent.UnitViews[unit] = unitObject;
                });
            }
        }
    }
}
