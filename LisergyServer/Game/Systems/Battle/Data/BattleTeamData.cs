﻿using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Systems.BattleGroup;
using Game.Systems.Battler;
using System;
using System.Runtime.InteropServices;

namespace Game.Systems.Battle.Data
{
    /// <summary>
    /// Represents static battle team data.
    /// Its the main input to start off battles.
    /// Will be present as an updated output from battles as well.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BattleTeamData
    {
        public GameId EntityId;
        public GameId OwnerID;
        public UnitGroup Units;

        public BattleTeamData(IEntity entity)
        {
            EntityId = entity.EntityId;
            OwnerID = entity.OwnerID;
            Units = entity.Get<BattleGroupComponent>().Units;
        }

        public BattleTeamData(params Unit[] units)
        {
            EntityId = GameId.ZERO;
            OwnerID = GameId.ZERO;
            Units = new UnitGroup(units);
        }

        public override string ToString()
        {
            return $"<BattleTeamData Owner={OwnerID} Entity={EntityId} Units={Units}>";
        }
    }
}
