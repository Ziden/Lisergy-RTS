using Game.Events;
using Game.Inventories;
using System;
using System.Collections.Generic;
using System.Linq;
using Game.Movement;
using Game.Battles;
using Game.Events.GameEvents;
using Game.Battle;
using Game.Events.ServerEvents;

namespace Game.Entity
{
    [Serializable]
    public class Party : MovableWorldEntity, IBattleable
    {
        [NonSerialized]
        private const int PARTY_SIZE = 4;

        protected string _battleID;
        private byte _partyIndex;
        private Unit[] _units = new Unit[PARTY_SIZE] { null, null, null, null };

        [NonSerialized]
        private EntityCargo _cargo;

        public bool IsAlive() => _units.Where(u => u != null && u.Stats.HP > 0).Any();

        public override TimeSpan GetMoveDelay() => TimeSpan.FromSeconds(0.25);

        public bool CanMove()
        {
            return !IsBattling;
        }

        [NonSerialized]
        private DateTime _lastBattleTime;

        public virtual string BattleID
        {
            get => _battleID;
            set
            {
                if (value != null)
                {
                    _lastBattleTime = DateTime.Now;
                }
                _battleID = value;
            }
        }

        public bool IsBattling => _battleID != null;

        public Party(PlayerEntity owner, byte partyIndex) : base(owner)
        {
            _partyIndex = partyIndex;
            _cargo = new EntityCargo(this);
        }

        public Party(PlayerEntity owner) : base(owner)
        {
            _cargo = new EntityCargo(this);
        }

        public byte PartyIndex { get => _partyIndex; set => _partyIndex = value; }

        public override Tile Tile
        {
            get => base.Tile;
            set
            {
                base.Tile = value;
                if (value != null && value.StaticEntity != null)
                {
                    if (this.Course != null && this.Course.Intent == MovementIntent.Offensive && this.Course.IsLastMovement())
                    {
                        Tile.Game.GameEvents.Call(new OffensiveMoveEvent()
                        {
                            Defender = value.StaticEntity,
                            Attacker = this
                        });
                    }
                }
            }
        }

        public override byte GetLineOfSight()
        {
            return _units.Where(u => u != null).Select(u => StrategyGame.Specs.Units[u.SpecId].LOS).Max();
        }

        public IEnumerable<Unit> GetValidUnits()
        {
            return _units.Where(u => u != null);
        }

        public Unit[] GetUnits()
        {
            return _units;
        }

        public virtual void SetUnit(Unit u, int index)
        {
            _units[index] = u;
        }

        public virtual void AddUnit(Unit u)
        {
            if (u.Party != null && u.Party != this)
                u.Party.RemoveUnit(u);
            var freeIndex = Array.IndexOf(_units, null);
            SetUnit(u, freeIndex);
        }

        public virtual void SetUnits(Unit [] units)
        {
            if (units.Length != PARTY_SIZE) throw new Exception("Party size not " + PARTY_SIZE);
            this._units = units;
        }

        public virtual void RemoveUnit(Unit u)
        {
            var index = Array.IndexOf(_units, u);
            _units[index] = null;
        }

        public BattleTeam GetBattleTeam()
        {
            return new BattleTeam(this, this._units);
        }

        public void OnBattleFinished(TurnBattle battle, BattleHeader BattleHeader, BattleTurnEvent[] Turns)
        {
            this.BattleID = null;
            if (!IsAlive())
            {
                var center = this.Owner.GetCenter();
                this.Tile = center.Tile;
                foreach (var unit in _units)
                    unit?.HealAll();
            }
        }

        public override string ToString()
        {
            return $"<Party Battling={IsBattling} Id={Id} Index={PartyIndex} Owner={OwnerID}>";
        }

        public void OnBattleStarted(TurnBattle battle)
        {
            this.BattleID = battle.ID.ToString();
        }

        public ServerEvent GetUpdatePacket() => new PartyStatusUpdatePacket(this);
    }
}
