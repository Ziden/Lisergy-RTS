using Game.DataTypes;
using Game.Entity;
using Game.Systems.Party;
using Game.Systems.Tile;
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

        [NonSerialized]
        public string Name;

        public GameId Id { get; protected set; }
        public ushort SpecId { get; private set; }
        private UnitStats _statsData;

        public Unit(ushort unitSpecId)
        {
            Name = "NoName";
            SpecId = unitSpecId;
            Id = Guid.NewGuid();
            _statsData.SetStats(UnitStats.DEFAULT);
        }

        public ref byte Atk { get => ref _statsData.Atk;  }
        public ref byte Def { get => ref _statsData.Def; }
        public ref byte Matk { get => ref _statsData.Matk;  }
        public ref byte Mdef { get => ref _statsData.Mdef;  }
        public ref byte Speed { get => ref _statsData.Speed;  }
        public ref byte Accuracy { get => ref _statsData.Accuracy; }
        public ref byte Weight { get => ref _statsData.Weight; }
        public ref byte Move { get => ref _statsData.Weight; }
        public ref ushort HP { get => ref _statsData.HP; }
        public ref ushort MaxHP { get => ref _statsData.MaxHP; }
        public ref ushort MP { get => ref _statsData.MP; }
        public ref ushort MaxMP { get => ref _statsData.MaxMP; }

        public void ModifyStats(Dictionary<Stat, ushort> stats)
        {
            foreach (KeyValuePair<Stat, ushort> kp in stats)
            {
                _statsData.AddStat(kp.Key, kp.Value);
            }
        }

        public void ModifyStat(Stat stat, in ushort value)
        {
            _statsData.AddStat(stat, value);
        }

        public Unit SetBaseStats()
        {
            _statsData.SetStats(GameLogic.Specs.Units[SpecId].Stats);
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
            return HashCode.Combine(SpecId, _statsData, _partyId);
        }

        public int GetHashCode(Unit obj)
        {
            return obj.GetHashCode();
        }
    }
}
