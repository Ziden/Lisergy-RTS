using Assets.Code.Views;
using Game.Battler;
using Game.ECS;
using Game.Party;
using System.Collections.Generic;
using Assets.Code.Code.Utils;
using Game.Events.GameEvents;
using UnityEngine;

namespace Assets.Code.World
{
    public partial class PartyView : EntityView<PartyEntity>,IMovementLogic
    {
        private static GameObject _container;

        private Dictionary<Unit, UnitView> _unitObjects = new Dictionary<Unit, UnitView>();
        public override PartyEntity Entity { get; }
        public override GameObject GameObject { get; set; }

        public IReadOnlyDictionary<Unit, UnitView> UnitObjects => _unitObjects;

        public PartyView(PartyEntity party)
        {
            Entity = party;
        }

        public override bool Instantiated => GameObject != null;

        public override void OnUpdate(PartyEntity partyFromNetwork, List<IComponent> syncedComponents)
        {
            Entity.Tile = GameView.World.GetTile(partyFromNetwork);
          
            if (Instantiated) return;
            RegisterEvents();
            Instantiate();
            CreateUnitObjects();
        }

        public override void Instantiate()
        {
            GameObject = new GameObject($"{Entity.OwnerID}-{Entity.Id}");
            GameObject.transform.SetParent(Container.transform);
            GameObject.transform.position = Entity.Tile.Position(0.2f);
            GameObject.SetActive(true);

            StackLog.Debug($"Created new party instance {this}");
        }

        private static GameObject Container
        {
            get
            {
                if (_container == null)
                    _container = new GameObject("Parties");
                return _container;
            }
        }

        public void CreateUnitObjects()
        {
            foreach (var unit in Entity.BattleGroupLogic.GetValidUnits())
            {
                var unitObject = new UnitView(unit);
                unitObject.AddToScene();
                unitObject.GameObject.transform.SetParent(GameObject.transform);
                unitObject.GameObject.transform.localPosition = Vector3.zero;
                _unitObjects[unit] = unitObject;
            }
        }
        
        
    }
}
