using Assets.Code.Entity;
using Game;
using Game.DataTypes;
using Game.ECS;
using Game.Entity;
using Game.Entity.Components;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.World
{

    public static class ClientPartyExt
    {
        public static Unit[] GetClientUnits(this Party p)
        {
            return p.GetUnits().Select(u => u != null ? new ClientUnit(u) : null).ToArray();
        } 
    }

    public class ClientParty : Party, IGameObject, IClientEntity<Party, ClientParty>
    {
        private static GameObject _container;

        public ClientParty(PlayerEntity owner) : base(owner)
        {

        }

        public GameObject GameObject { get; set; }
        public Tile ClientTile { get => this.Tile; }

        public ClientParty UpdateData(Party partyFromNetwork, List<IComponent> syncedComponents)
        {    
            _id = partyFromNetwork.Id;
            SyncComponents(syncedComponents);
            Owner.Parties[PartyIndex] = this;
            Id = partyFromNetwork.Id;
            if(GameObject != null)
                Tile = GameView.World.GetTile(partyFromNetwork);
            return this;
        }

        private void SyncComponents(List<IComponent> syncedComponents)
        {
            foreach(var c in syncedComponents)
            {
                if(!this.Components.Has(c.GetType())) {
                    this.Components.Add(c);
                } 
                Components.Get(c.GetType()).UpdateFrom(c);
                if(c is BattleGroupComponent bg)
                {
                    var frontLine = bg.FrontLine();
                    List<Unit> newUnits = new List<Unit>();
                    foreach(var unit in frontLine)
                    {
                        newUnits.Add(new ClientUnit(unit));
                    }
                    UpdateUnits(newUnits);
                }
            }
        }

        public void InstantiateInScene(Party partyFromNetwork)
        {
            GameObject = new GameObject($"{partyFromNetwork.OwnerID}-{Id}");
            GameObject.transform.SetParent(Container.transform);
            Tile = GameView.World.GetTile(partyFromNetwork);
            GameObject.SetActive(true);
            Render();
            StackLog.Debug($"Created new party instance {this}");
        }

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
                    StackLog.Debug($"Moving {this} gameobject to {value}");
                    GameObject.transform.position = new Vector3(value.X, 0.1f, value.Y);
                }
                ClientEvents.PartyFinishedMove(this, old, base.Tile);
            }
        }

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

        private static GameObject Container
        {
            get
            {
                if (_container == null)
                    _container = new GameObject("Parties");
                return _container;
            }
        }

        public bool IsMine()
        {
            return this.OwnerID == MainBehaviour.Player.UserID;
        }

        public override void AddUnit(Unit u)
        {
            base.AddUnit(u);
        }

        public void Render()
        {
            foreach (var unit in GetValidUnits())
            {
                var clientUnit = unit as ClientUnit;
                clientUnit.AddToScene();
                clientUnit.GameObject.transform.SetParent(GameObject.transform);
                clientUnit.GameObject.transform.localPosition = Vector3.zero;
            }
        }
    }
}
