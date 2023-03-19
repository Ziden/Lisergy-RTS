using Assets.Code.Entity;
using Assets.Code.Views;
using Game;
using Game.DataTypes;
using Game.ECS;
using Game.Entity;
using Game.Entity.Components;
using Game.Entity.Entities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.World
{
    public partial class PartyView : EntityView<PartyEntity>
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
            Entity.Owner.Parties[Entity.PartyIndex] = Entity;
            if (!Instantiated)
            {
                Instantiate();
                CreateUnitObjects();
                RegisterEvents();
            }
            Entity.Tile = GameView.World.GetTile(partyFromNetwork);
        }

        public override void Instantiate()
        {
            GameObject = new GameObject($"{Entity.OwnerID}-{Entity.Id}");
            GameObject.transform.SetParent(Container.transform);
            GameObject.SetActive(true);
            StackLog.Debug($"Created new party instance {this}");
        }

        /*
        public override Tile Tile
        {
            get => base.Tile;
            set
            {
                var old = base.Tile;
                if (value != null && !BattleID.IsZero())
                    Effects.BattleEffect(value as Tile);

                base.Tile = value;
                if (value != null)
                {
                    GameObject.transform.position = new Vector3(value.X, 0.1f, value.Y);
                }
                ClientEvents.PartyFinishedMove(this, old, base.Tile);
            }
        }
        */

        /*
        public override GameId BattleID
        {
            get => base.BattleID; set {

                if(this.Tile != null && !value.IsZero() && BattleID.IsZero())
                    Effects.BattleEffect(this.Tile);
                if (!this.BattleID.IsZero() && value.IsZero())
                    Effects.StopEffect(this.Tile);
                base.BattleID = value;
            }
        }
        */

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
            foreach (var unit in Entity.BattleLogic.GetValidUnits())
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
