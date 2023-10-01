using Game.DataTypes;
using Game.Entity;
using Game.Systems.Party;
using GameData.buffs;
using System;
using System.Collections.Generic;

namespace Game.Systems.Battler
{
    [Serializable]
    public unsafe class Unit : IEquatable<Unit>, IEqualityComparer<Unit>
    {
        [NonSerialized]
        private byte _partyId = byte.MaxValue;

        [NonSerialized]
        private PartyEntity _party;

        [field: NonSerialized]
        public string Name { get; set; }
        public GameId Id { get; protected set; }
        public ushort SpecId { get; private set; }

        private UnitStats _statsData;

        public UnitStats Stats => _statsData;

        public Unit(ushort unitSpecId)
        {
            Name = "NoName";
            SpecId = unitSpecId;
            Id = Guid.NewGuid();
            _statsData = UnitStats.DEFAULT;
        }

        public byte Atk { get => _statsData.Atk; set => _statsData.Atk = value; }
        public byte Def { get => _statsData.Def; set => _statsData.Def = value; }
        public byte Matk { get => _statsData.Matk; set => _statsData.Matk = value; }
        public byte Mdef { get => _statsData.Mdef; set => _statsData.Mdef = value; }
        public byte Speed { get => _statsData.Speed; set => _statsData.Speed = value; }
        public byte Accuracy { get => _statsData.Accuracy; set => _statsData.Accuracy = value; }
        public byte Weight { get => _statsData.Weight; set => _statsData.Weight = value; }
        public byte Move { get => _statsData.Weight; set => _statsData.Weight = value; }
        public ushort HP { get => _statsData.HP; set => _statsData.HP = value; }
        public ushort MaxHP { get => _statsData.MaxHP; set => _statsData.MaxHP = value; }
        public ushort MP { get => _statsData.MP; set => _statsData.MP = value; }
        public ushort MaxMP { get => _statsData.MaxMP; set => _statsData.MaxMP = value; }


        public void ModifyStats(Dictionary<Stat, ushort> stats)
        {
            foreach (KeyValuePair<Stat, ushort> kp in stats)
            {
                _statsData.AddStat(kp.Key, kp.Value);
            }
        }

        public void ModifyStat(Stat stat, ushort value)
        {
            _statsData.AddStat(stat, value);
        }

        public Unit SetBaseStats()
        {
            _statsData.SetStats(StrategyGame.Specs.Units[SpecId].Stats);
            HealAll();
            return this;
        }

        public void HealAll()
        {
            HP = MaxHP;
            MP = MaxMP;
        }

        public PartyEntity Party
        {
            get => _party;
            set
            {
                _party = value;
                _partyId = value != null ? value.PartyIndex : byte.MaxValue;
            }
        }

        public byte PartyId => _partyId;

        public override string ToString()
        {
            return $"<Unit name={Name} spec={SpecId}/>";
        }

        public bool Equals(Unit other)
        {
            return other != null && _partyId == other._partyId && SpecId == other.SpecId && _statsData.Equals(other._statsData);
        }

        public bool Equals(Unit x, Unit y)
        {
            return x != null && x.Equals(y);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SpecId, Stats, _partyId);
        }

        public int GetHashCode(Unit obj)
        {
            return obj.GetHashCode();
        }
    }
}
