using Game.Entity;
using Game.Systems.BattleGroup;
using Game.Systems.Battler;
using System;

namespace Game.Battle.Data
{
    /// <summary>
    /// Controls battle memory for faster data manipulation.
    /// Will allocate and free for every battle, and will re-use previously allocated memory 
    /// if it was free'd before thus not requiring extra allocation.
    /// All battle memory is allocated on the stack.
    /// </summary>
    internal unsafe class BattleTeamMemory
    {
        private readonly Unit* _units;
        private readonly BattleUnitState* _unitStates;

        public BattleTeamMemory(in BattleTeamData originalData)
        {
            var groupSize = sizeof(Unit) * 4;
            if (groupSize != sizeof(UnitGroup)) throw new Exception("UnitGroup size needs to be 4 units size");
            _units = (Unit*)UnmanagedMemory.Alloc(groupSize);
            _unitStates = (BattleUnitState*)UnmanagedMemory.Alloc(sizeof(BattleUnitState) * 4);
            fixed (UnitGroup* group = &originalData.Units)
            {
                Buffer.MemoryCopy(group, _units, groupSize, groupSize);
            }
        }

        public void FreeAndCopyResults(ref BattleTeamData originalData)
        {
            var groupSize = sizeof(Unit) * 4;
            fixed (UnitGroup* group = &originalData.Units)
            {
                Buffer.MemoryCopy(_units, group, groupSize, groupSize);
            }
            UnmanagedMemory.FreeForReuse((IntPtr)_unitStates);
            UnmanagedMemory.FreeForReuse((IntPtr)_units);
        }

        public IntPtr Pointer => (IntPtr)_units;

        public Unit* GetUnit(in int index) => _units + index;
        public BattleUnitState* GetUnitState(in int index) => _unitStates + index;
    }
}
