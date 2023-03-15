using Game.Entity;
using GameData.buffs;
using GameData.Specs;
using System;
using System.Collections.Generic;

namespace Game
{
    [Serializable]
    public unsafe class Unit
    {
        [NonSerialized]
        private byte _partyId = byte.MaxValue;

        [NonSerialized]
        private Party _party;

        [field: NonSerialized]
        public string Name { get; set; }
        public GameId Id { get; protected set; }
        public ushort SpecId { get; private set; }

        private UnitStats _statsData;

        public UnitStats Stats => _statsData;

        private UnitStats* _stats
        {
            get
            {
                fixed (UnitStats* ptr = &_statsData)
                {
                    return ptr;
                }
            }
        }

        public Unit(ushort unitSpecId)
        {
            this.Name = "NoName";
            this.SpecId = unitSpecId;
            this.Id = Guid.NewGuid();
            _statsData = UnitStats.DEFAULT;
        }

        public byte Atk { get => _stats->Atk; set => _stats->Atk = value; }
        public byte Def { get => _stats->Def; set => _stats->Def = value; }
        public byte Matk { get => _stats->Matk; set => _stats->Matk = value; }
        public byte Mdef { get => _stats->Mdef; set => _stats->Mdef = value; }
        public byte Speed { get => _stats->Speed; set => _stats->Speed = value; }
        public byte Accuracy { get => _stats->Accuracy; set => _stats->Accuracy = value; }
        public byte Weight { get => _stats->Weight; set => _stats->Weight = value; }
        public byte Move { get => _stats->Weight; set => _stats->Weight = value; }
        public ushort HP { get => _stats->HP; set => _stats->HP = value; }
        public ushort MaxHP { get => _stats->MaxHP; set => _stats->MaxHP = value; }
        public ushort MP { get => _stats->MP; set => _stats->MP = value; }
        public ushort MaxMP { get => _stats->MaxMP; set => _stats->MaxMP = value; }


        public void ModifyStats(Dictionary<Stat, ushort> stats)
        {
            foreach (var kp in stats)
            {
                _stats->AddStat(kp.Key, kp.Value);
            }
        }

        public void ModifyStat(Stat stat, ushort value)
        {
            _stats->AddStat(stat, value);
        }

        public void SetSpecStats()
        {
            _stats->SetStats(StrategyGame.Specs.Units[this.SpecId].Stats);
            HealAll();
        }

        public void HealAll()
        {
            this.HP = this.MaxHP;
            this.MP = this.MaxMP;
        }

        public Party Party
        {
            get => _party;
            set
            {
                _party = value;
                if (value != null)
                    _partyId = value.PartyIndex;
                else
                    _partyId = byte.MaxValue;
            }
        }

        public byte PartyId { get => _partyId; }

        public override string ToString()
        {
            return $"<Unit name={Name} spec={SpecId}/>";
        }
    }
}
