﻿using Game.DataTypes;
using Game.ECS;
using Game.Events.Bus;
using Game.Events.GameEvents;
using Game.Systems.Battler;
using Game.Systems.FogOfWar;
using Game.Systems.Map;

namespace Game.Systems.Party
{
    public class PartySystem : GameSystem<PartyComponent>
    {
        public PartySystem(LisergyGame game) : base(game) { }

        public override void OnEnabled()
        {
            EntityEvents.On<GroupDeadEvent>(OnGroupDead);
        }

        private void OnGroupDead(IEntity e, GroupDeadEvent ev)
        {
            e.EntityLogic.BattleGroup.Heal();
            var center = Players.GetPlayer(e.OwnerID).GetCenter().Get<MapPlacementComponent>();
            e.EntityLogic.Map.SetPosition(Game.GameWorld.GetTile(center.Position));
        }
    }
}