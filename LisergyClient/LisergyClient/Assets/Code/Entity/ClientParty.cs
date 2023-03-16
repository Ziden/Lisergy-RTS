using Game;
using Game.Entity;
using System.Linq;
using UnityEngine;

namespace Assets.Code.World
{

    public static class ClientPartyExt
    {
        public static ClientUnit[] GetClientUnits(this Party p)
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

        private GameObject _gameObject;
        public ClientTile ClientTile { get => (ClientTile)this.Tile; }

        public ClientParty UpdateData(Party partyFromNetwork)
        {    
            _id = partyFromNetwork.Id;
            BattleID = partyFromNetwork.BattleID;
            SetUnits(partyFromNetwork.GetClientUnits());
            PartyIndex = partyFromNetwork.PartyIndex;
            Owner.Parties[PartyIndex] = this;
            Id = partyFromNetwork.Id;
            Tile = ClientStrategyGame.ClientWorld.GetClientTile(partyFromNetwork);
            return this;
        }

        public void InstantiateInScene(Party partyFromNetwork)
        {
            _gameObject = new GameObject($"{partyFromNetwork.OwnerID}-{Id}-{partyFromNetwork.PartyIndex}");
            _gameObject.transform.SetParent(Container.transform);
            UpdateData(partyFromNetwork);
            this.GetGameObject().SetActive(true);
            Render();
            StackLog.Debug($"Created new party instance {this}");
        }

        public GameObject GetGameObject() => _gameObject;

        public override Tile Tile
        {
            get => base.Tile;
            set
            {
                var old = base.Tile;
                if (value != null && !BattleID.IsZero())
                    Effects.BattleEffect(value as ClientTile);

                base.Tile = value;
                if (value != null)
                {
                    StackLog.Debug($"Moving {this} gameobject to {value}");
                    _gameObject.transform.position = new Vector3(value.X, 0.1f, value.Y);
                }
                ClientEvents.PartyFinishedMove(this, (ClientTile)old, (ClientTile)base.Tile);
            }
        }

        public override GameId BattleID
        {
            get => base.BattleID; set {

                if(this.Tile != null && !value.IsZero() && BattleID.IsZero())
                    Effects.BattleEffect(this.Tile as ClientTile);
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
                clientUnit.GetGameObject().transform.SetParent(_gameObject.transform);
                clientUnit.GetGameObject().transform.localPosition = Vector3.zero;
            }
        }
    }
}
