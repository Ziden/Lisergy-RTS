using Game.DataTypes;
using Game.Entity;
using GameData.Specs;
using System;
using System.Collections.Generic;

namespace Game.Systems.Battler
{
    [Serializable]
    public class Unit : IEquatable<Unit>, IEqualityComparer<Unit>
    {
        public GameId Id { get; protected set; }
        public ushort SpecId { get; private set; }
        private UnitStats _statsData;

        public Unit(UnitSpec spec)
        {
            Id = Guid.NewGuid();
            SpecId = spec.UnitSpecID;
            _statsData.SetStats(spec.Stats);
            HealAll();
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

        public void HealAll()
        {
            HP = MaxHP;
            MP = MaxMP;
        }

        public override string ToString()
        {
            return $"<Unit Spec={SpecId}/>";
        }

        public bool Equals(Unit other)
        {
            return other != null && SpecId == other.SpecId && _statsData.Equals(other._statsData);
        }

        public bool Equals(Unit x, Unit y)
        {
            return x != null && x.Equals(y);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SpecId, _statsData);
        }

        public int GetHashCode(Unit obj)
        {
            return obj.GetHashCode();
        }
    }
}
