using System;
using System.Collections.Generic;
using System.Linq;
using Game.Systems.Battler;

namespace Game.Systems.BattleGroup
{
    /// <summary>
    ///     Unmanaged reference of a unit group
    /// </summary>
    [Serializable]
	public class UnitGroup : IComparable<UnitGroup>, IEqualityComparer<UnitGroup>
	{
		public List<Unit> Group = new List<Unit>();

		public UnitGroup()
		{
		}

		public UnitGroup(Unit[] units)
		{
			Group = units.ToList();
		}

		public Unit[] Array => Group.ToArray();

		public bool AllDead => Group.All(u => u.Stats.HP <= 0);

        /// <summary>
        ///     Gets amount of valid (non null) units in a group
        /// </summary>
        public int Valids => Group.Count(u => u?.Valid ?? false);

        /// <summary>
        ///     Checks if a group has no units
        /// </summary>
        public bool Empty => Group.All(u => !u?.Valid ?? false);

		public Unit this[int x]
		{
			get => Group[x];
			set => Group[x] = value;
		}

		public int CompareTo(UnitGroup other)
		{
			return Array.SequenceEqual(other.Array) ? 1 : 0;
		}

		public bool Equals(UnitGroup x, UnitGroup y)
		{
			return x.Array.SequenceEqual(y.Array);
		}

		public int GetHashCode(UnitGroup obj)
		{
			return obj.GetHashCode(this);
		}

		public bool Contains(in Unit u)
		{
			return Group.Contains(u);
		}

		public void HealAll()
		{
			foreach (var u in Group) u.HealAll();
		}

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

		public void Add(in Unit u)
		{
			Group.Add(u);
		}

		public IEnumerator<Unit> GetEnumerator()
		{
			return Group.GetEnumerator();
		}

		public override string ToString()
		{
			return $"{string.Join(" ", Group.Where(u => u != null && u.Valid))}";
		}
	}
}