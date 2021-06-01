using Game.Events;
using Game.Inventories;
using System;
using System.Collections.Generic;
using System.Linq;
using Game.Movement;
using Game.Battles;

namespace Game.Entity
{
    [Serializable]
    public class Party : MovableWorldEntity
    {
        private byte _partyIndex;
        private Unit[] _units = new Unit[4] { null, null, null, null };

        private Inventory _cargo = new Inventory();

        public byte PartyIndex { get => _partyIndex; }
        public override TimeSpan GetMoveDelay() => TimeSpan.FromSeconds(0.25);

        public bool CanMove()
        {
            return !IsBattling;
        }

        public Party(PlayerEntity owner, byte partyIndex) : base(owner)
        {
            _partyIndex = partyIndex;
        }

        public void StartBattle(BattleTeam enemy)
        {
            var playerTeam = new BattleTeam(this.Owner, this._units);
            _battleID = Guid.NewGuid().ToString();

            // TODO: Make this better
            Tile.Chunk.Map.World.Game.NetworkEvents.RunCallbacks(new BattleStartEvent()
            {
                X = this.X,
                Y = this.Y,
                Attacker = playerTeam,
                Defender = enemy,
                BattleID = _battleID
            });
        }

        public override Tile Tile
        {
            get => base.Tile;
            set
            {
                base.Tile = value;
                if (value != null && value.StaticEntity is Dungeon)
                {
                    if (this.Course != null && this.Course.Intent == MovementIntent.Offensive && this.Course.IsLastMovement())
                    {
                        var enemyTeam = ((Dungeon)value.StaticEntity).Battles.First();
                        Log.Info($"{this} did an offensive move triggering a battle with {enemyTeam}");
                        StartBattle(enemyTeam);
                    }
                }
            }
        }

        public virtual string BattleID { get => _battleID; set => _battleID = value; }

        public override byte GetLineOfSight()
        {
            return _units.Where(u => u != null).Select(u => StrategyGame.Specs.Units[u.SpecId].LOS).Max();
        }

        public IEnumerable<Unit> GetUnits()
        {
            return _units.Where(u => u != null);
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

        public virtual void RemoveUnit(Unit u)
        {
            var index = Array.IndexOf(_units, u);
            _units[index] = null;
        }

        public override string ToString()
        {
            return $"<Party Battling={IsBattling} Id={Id} Index={PartyIndex} Owner={OwnerID}>";
        }
    }
}
