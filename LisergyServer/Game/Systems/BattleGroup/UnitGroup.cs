using Game.Systems.Battler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Game.Systems.BattleGroup
{
    /// <summary>
    /// Unmanaged reference of a unit group
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UnitGroup : IEnumerable<Unit>, IComparable<UnitGroup>, IEqualityComparer<UnitGroup>
    {
        public Unit Leader;
        public Unit LeftFlank;
        public Unit RightFlank;
        public Unit Rear;

        public Unit[] Array => new Unit[] { Leader, LeftFlank, RightFlank, Rear };

        public bool Contains(in Unit u)
        {
            return Leader == u || LeftFlank == u || RightFlank == u || Rear == u;
        }

        public void HealAll()
        {
            Leader.HealAll();
            LeftFlank.HealAll();
            RightFlank.HealAll();
            Rear.HealAll();
        }

        public bool AllDead => Leader.HP == 0 && LeftFlank.HP == 0 && RightFlank.HP == 0 && Rear.HP == 0;

        public bool Remove(in Unit u)
        {
            var i = IndexOf(u);
            if (i == -1) return false;
            this[i] = default;
            return true;
        }

        public int IndexOf(in Unit u)
        {
            if (Leader == u) return 0;
            else if (LeftFlank == u) return 1;
            else if (RightFlank == u) return 2;
            else if (Rear == u) return 3;
            return -1;
        }

        /// <summary>
        /// Gets amount of valid (non null) units in a group
        /// </summary>
        public int Valids => this.Count(u => u.Valid);

        /// <summary>
        /// Checks if a group has no units
        /// </summary>
        public bool Empty => this.All(u => !u.Valid);

        public Unit this[int x]
        {
            get
            {
                switch (x)
                {
                    case 0: return Leader;
                    case 1: return LeftFlank;
                    case 2: return RightFlank;
                    case 3: return Rear;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (x)
                {
                    case 0: Leader = value; break;
                    case 1: LeftFlank = value; break;
                    case 2: RightFlank = value; break;
                    case 3: Rear = value; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public void Add(in Unit u)
        {
            for (var x = 0; x < 4; x++)
            {
                var existing = this[x];
                if (!existing.Valid)
                {
                    this[x] = u;
                    return;
                }
            }
            throw new IndexOutOfRangeException();
        }

        public IEnumerator<Unit> GetEnumerator()
        {
            if (Leader.Valid) yield return Leader;
            if (LeftFlank.Valid) yield return LeftFlank;
            if (RightFlank.Valid) yield return RightFlank;
            if (Rear.Valid) yield return Rear;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public UnitGroup(Unit[] units)
        {
            Leader = units[0];
            if (units.Length > 1) LeftFlank = units[1];
            else LeftFlank = default;
            if (units.Length > 2) RightFlank = units[2];
            else RightFlank = default;
            if (units.Length > 3) Rear = units[3];
            else Rear = default;
        }

        public override string ToString()
        {
            return $"{string.Join(" ", this.Where(u => u.Valid))}";
        }

        public int CompareTo(UnitGroup other)
        {
            return this.Array.SequenceEqual(other.Array) ? 1 : 0;
        }

        public bool Equals(UnitGroup x, UnitGroup y)
        {
            return x.Array.SequenceEqual(y.Array);
        }

        public int GetHashCode(UnitGroup obj)
        {
            return obj.GetHashCode(this);
        }
    }
}
