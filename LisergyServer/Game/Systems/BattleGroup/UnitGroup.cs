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
    public class UnitGroup : IEnumerable<Unit>, IComparable<UnitGroup>, IEqualityComparer<UnitGroup>
    {
        public List<Unit> Group = new List<Unit>();

        public Unit[] Array => Group.ToArray();

        public bool Contains(in Unit u)
        {
            return Group.Contains(u);
        }

        public void HealAll()
        {
            foreach (var u in Group) {
                u.HealAll();
            }
        }

        public bool AllDead => Group.All(u => u.Stats.HP <= 0);

        public bool Remove(in Unit u)
        {
            var i = IndexOf(u);
            if (i == -1) return false;
            this[i] = default;
            return true;
        }

        public int IndexOf(in Unit u)
        {
            return Group.IndexOf(u);
        }

        /// <summary>
        /// Gets amount of valid (non null) units in a group
        /// </summary>
        public int Valids => this.Count(u => u?.Valid ?? false);

        /// <summary>
        /// Checks if a group has no units
        /// </summary>
        public bool Empty => this.All(u => !u?.Valid ?? false);

        public Unit this[int x]
        {
            get
            {
                return Group[x];
            }
            set
            {
                Group[x] = value;
            }
        }

        public void Add(in Unit u)
        {
            Group.Add(u);
        }

        public IEnumerator<Unit> GetEnumerator()
        {
            return Group.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Group.GetEnumerator();
        }

        public UnitGroup()
        {
        }

        public UnitGroup(Unit[] units)
        {
            Group = units.ToList();
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
