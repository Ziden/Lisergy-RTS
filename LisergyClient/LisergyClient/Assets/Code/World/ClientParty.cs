using Game;
using Game.Entity;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientParty : Party, IGameObject
    {
        private static GameObject _container;

        private GameObject _gameObject;
        public ClientTile ClientTile { get => (ClientTile)this.Tile; }

        public ClientParty Update(Party partyFromNetwork)
        {
            _id = partyFromNetwork.Id;
            _x = (ushort)partyFromNetwork.X;
            _y = (ushort)partyFromNetwork.Y;
            BattleID = partyFromNetwork.BattleID;
            for (var i = 0; i < 4; i++)
                this.SetUnit(null, i);
            foreach (var unit in partyFromNetwork.GetUnits())
                this.AddUnit(new ClientUnit(unit));
            return this;
        }

        public ClientParty(PlayerEntity owner, Party partyFromNetwork) : base(owner, partyFromNetwork.PartyIndex)
        {
            _gameObject = new GameObject($"{owner.UserID}-{Id}-{partyFromNetwork.PartyIndex}");
            _gameObject.transform.SetParent(Container.transform);
            Update(partyFromNetwork);
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
                if (value != null && BattleID != null)
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

        public override string BattleID
        {
            get => base.BattleID; set {

                if(this.Tile != null && value != null && BattleID == null)
                    Effects.BattleEffect(this.Tile as ClientTile);
                if (this.BattleID != null && value == null)
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
            foreach (var unit in GetUnits())
            {
                var unitObject = ((ClientUnit)unit).AddToScene();
                unitObject.transform.SetParent(_gameObject.transform);
            }
        }


    }
}
